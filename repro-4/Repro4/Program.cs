namespace Repro
{
    using System;
    using System.Configuration;
    using Acme.Commands;
    using Castle.Windsor;
    using NServiceBus;
    using NServiceBus.Installation.Environments;

    static class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            var configuration = Configure.With()
                .CastleWindsorBuilder(container)
                .DefiningCommandsAs(type =>
                    type.Namespace != null
                    && type.Namespace.EndsWith(".Commands")
                    && type.Namespace.StartsWith("Acme."))
                .DefiningEventsAs(type =>
                    type.Namespace != null
                    && type.Namespace.StartsWith("Acme.")
                    && type.Namespace.EndsWith(".Events"))
                .DefiningMessagesAs(
                    type => (type.Namespace != null && type.Namespace.EndsWith(".Messages") && type.Namespace.StartsWith("Acme."))
                    ||
                    (type.Namespace != null && type.Namespace.EndsWith(".Internal") && type.Namespace.StartsWith("Acme."))
                    ||
                    (type.Namespace != null && type.Namespace.EndsWith(".OutboundMessages")))
                .DefiningDataBusPropertiesAs(type => type.Name.EndsWith("Bilagor") || type.Name.EndsWith("Protokoll"))
                .FileShareDataBus(ConfigurationManager.AppSettings["DataBusPath"])
                .UseTransport<Msmq>()
                .PurgeOnStartup(false)
                .UseNHibernateSagaPersister()
                .UseInMemoryGatewayPersister()
                .UseNHibernateTimeoutPersister()
                .UseNHibernateSubscriptionPersister()
                .UnicastBus()
                .RunHandlersUnderIncomingPrincipal(false);

            //AutoMapperConfiguration.Configure();

            var bus = configuration.UnicastBus().CreateBus().Start(() => configuration.ForInstallationOn<Windows>().Install());

            Console.WriteLine(
@"End point started.

Commands:
  I     Send a message immediately.
  S     Defer a message by 3 seconds.
  H     Defer a message by 3 hours.

Ctrl+C to exit.");

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.I:
                        bus.Foo();
                        break;
                    case ConsoleKey.H:
                        bus.DeferFoo(CreateCommand(), TimeSpan.FromHours(3));
                        break;
                    case ConsoleKey.S:
                        bus.DeferFoo(CreateCommand(), TimeSpan.FromSeconds(3));
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }

        private static void Foo(this IBus bus)
        {
            var doFoo = CreateCommand();
            Console.WriteLine($"Sending {doFoo}...");
            bus.Send(doFoo);
        }

        private static DoFoo CreateCommand() => new DoFoo { Bar = "Baz", TimestampUtc = DateTimeOffset.UtcNow };

        private static void DeferFoo(this IBus bus, DoFoo doFoo, TimeSpan delay)
        {
            Console.WriteLine($"Deferring {doFoo} to {DateTimeOffset.Now + delay}...");
            bus.Defer(delay, doFoo);
        }
    }

    public class DoFooHandler : IHandleMessages<DoFoo>
    {
        public void Handle(DoFoo message) => Console.WriteLine($"Received {message}");
    }
}

namespace Acme.Commands
{
    using System;

    public class DoFoo
    {
        public DateTimeOffset TimestampUtc { get; set; }

        public string Bar { get; set; }

        public override string ToString() => $"{nameof(Bar)}: {Bar}, {nameof(TimestampUtc)}: {TimestampUtc} (Ticks: {TimestampUtc.Ticks})";
    }
}
