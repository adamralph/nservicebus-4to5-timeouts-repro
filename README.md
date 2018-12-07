## Preparing your machine

- [Install the pre-requisites](#install-the-pre-requisites)
- [Set up the databases](#set-up-the-databases)
- [Run the solutions](#run-the-solutions)

### Install the pre-requisites
   
#### LocalDB

* Download and run the [SQL Server 2017 Express installer](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)
* In the installer, select the "Download Media" option
* Under "WHICH PACKAGE WOULD YOU LIKE TO DOWNLOAD?", select LocalDB
* Download and run `SqlLocalDB.msi` from the download location

#### SQLCMD

* Install [ODBC Driver 13.1 for SQL Server](https://www.microsoft.com/en-us/download/details.aspx?id=53339) (this is a pre-requisite of SQLCMD).
* Install [Command Line Utilities 14.0 for SQL Server](https://www.microsoft.com/en-us/download/details.aspx?id=53591) (often referred to as "SQLCMD").

### Set up the databases

Open an **elevated** command prompt, navigate to your copy of this repo, and run:

```Batchfile
powershell -NoProfile -ExecutionPolicy unrestricted -File Setup-LocalDBInstance.ps1
```

When you no longer need to run the repro, run `Teardown-LocalDBInstance.ps1`.

### Run the solutions

#### repro-4

- Run the repro-4 solution
- Press I to confirm a message can be immediately sent and received
- Press S to confirm a message can be deferred by three seconds and received
- Press H to defer a message for three hours
- Press H to defer a message for three seconds and immediately exit the app, before it is received

#### repro-5

- Run the repro-5 solution
- Confirm that the message that was deferred three seconds is received
- Confirm that the app does not log any errors
