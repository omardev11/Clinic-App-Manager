# Clinic App Manager

Clinic App Manager is a layered clinic-management application with a .NET 8 Web API and a responsive HTML/CSS client. It demonstrates patient, doctor, appointment, medical-record, prescription, payment, authentication, and authorization workflows.

## Description

The server follows a three-layer architecture that separates API controllers, business rules, and Entity Framework Core data access. ASP.NET Core Identity and JWT bearer authentication protect the API, while policy-based authorization and login rate limiting provide additional safeguards.

> This is an educational project. It is not intended to store real medical or patient information without a full production security and compliance review.

## Features

- Patient, doctor, and user management
- Appointment scheduling and status tracking
- Medical records and prescribed medications
- Payment and payment-method management
- ASP.NET Core Identity roles and users
- JWT access and refresh-token workflows
- Owner-or-admin authorization policy
- Swagger/OpenAPI documentation in development
- Login rate limiting
- Entity Framework Core migrations for SQL Server
- Static responsive clinic website

## Technologies

- .NET 8 and ASP.NET Core Web API
- C#
- Entity Framework Core and SQL Server
- ASP.NET Core Identity
- JWT bearer authentication
- Swagger/OpenAPI
- HTML and CSS

## Project Structure

```text
Clinic-App-Manager/
|-- ClientSide/
|   `-- ClinicAppManager/            # Static HTML/CSS client
|-- ServerSide/
|   |-- ClinicAppManager/            # API, authentication, and controllers
|   |-- ClinicAppManagerBusinessLayer/
|   `-- ClinicAppManagerDataLayer/   # EF Core entities and migrations
|-- .gitignore
`-- README.md
```

## Getting Started

### Requirements

- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Optional: Visual Studio 2022

### Configure Local Secrets

The committed `appsettings.json` contains only safe local defaults. Store the JWT signing key and optional administrator seed account with .NET user secrets:

```powershell
cd ServerSide/ClinicAppManager
dotnet user-secrets init
dotnet user-secrets set "JWT:SecretKey" "replace-with-a-random-secret-of-at-least-32-characters"
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "replace-with-a-strong-password"
```

For a different database, override `ConnectionStrings:ClinicAppManager` with user secrets or environment variables.

### Build and Run

```powershell
dotnet restore ServerSide/ClinicAppManager/ClinicAppManager.sln
dotnet build ServerSide/ClinicAppManager/ClinicAppManager.sln --configuration Release
dotnet run --project ServerSide/ClinicAppManager/ClinicAppManager.csproj
```

When running in the Development environment, open the HTTPS Swagger URL shown in the console.

### Database Migrations

From `ServerSide/ClinicAppManagerDataLayer`, apply the included migrations after configuring the database connection:

```powershell
dotnet ef database update --startup-project ../ClinicAppManager
```

## Security and Privacy

- No real database, patient data, passwords, JWT keys, or production configuration is included.
- Secrets should be supplied through .NET user secrets, environment variables, or a managed secret store.
- The ZIP/deployment copy and all generated build output are excluded.
- Review authentication, authorization, logging, encryption, backups, and regulatory requirements before any production use.

