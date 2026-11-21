# Online Voting System

A Windows Forms desktop application built with an MVC-oriented structure and SQL Server persistence.

## Requirements

- Windows 10/11
- Visual Studio 2022 or .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio) or SQL Server

## Run the application

1. Open `OnlineVotingSystem.slnx` in Visual Studio.
2. If using a SQL Server instance other than LocalDB, edit `DatabaseSettings.ConnectionString` in `OnlineVotingSystem/Data/DatabaseSettings.cs`.
3. Build and run the `OnlineVotingSystem` project.
4. On first startup, the application creates the `OnlineVotingSystem` database, tables, demo accounts, one election, and two candidates.

Default LocalDB connection string:

```text
Server=(localdb)\\MSSQLLocalDB;Database=OnlineVotingSystem;Trusted_Connection=True;TrustServerCertificate=True;
```

## Demo accounts

| Username | Password | Role |
| --- | --- | --- |
| admin | admin123 | Admin |
| voter | voter123 | Voter |

The current classroom version exposes the shared voting workflow to both accounts. The role is displayed after login and can be used when expanding the administration screens.

## Features

- SQL Server database initialization and seed data
- Username/password authentication with SHA-256 password hashes
- Election and candidate display
- One vote per user per election, enforced in both controller logic and a database unique constraint
- Vote confirmation before recording
- Live election results
- MVC separation: `Model`, `View`, `Controller`, `Data`, and `Database`

## Project structure

- `Model` - domain records
- `View` - WinForms screens
- `Controller` - application workflows
- `Data` - SQL Server connection and repository
- `Database` - schema creation and seed data

This is an educational project and is not intended for use in a certified public election. Production deployments would additionally require audited identity verification, encryption/key management, access control, logging, availability, and independent security review.
