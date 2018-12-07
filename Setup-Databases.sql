USE [master]
GO

:setvar DatabaseName "nservicebus"
IF  NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'$(DatabaseName)')
CREATE DATABASE [$(DatabaseName)] ON ( NAME = N'$(DatabaseName)', FILENAME = N'$(UserPath)\$(DatabaseName).mdf' )
GO
