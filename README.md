# SkillHub

## Description

SkillHub is a training center platform designed to manage workshops and participant enrollments.

The application is being developed as a .NET solution following Clean Architecture principles. The platform will allow participants to register, log in, browse available workshops, enroll in workshops, manage their enrollments, and download enrollment certificates.

Employees will be able to manage workshops, review participants enrolled in workshops, import enrollments from Excel files, and access workshop reports.

The project is being developed as part of a .NET performance assessment.

## Technologies

The project is being developed using the following technologies:

- C#
- .NET 10
- ASP.NET Core
- Clean Architecture
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Authentication
- Angular
- Excel processing
- PDF generation

> Additional technologies will be configured and documented as the corresponding layers and features are implemented.

## Architecture

The solution follows a Clean Architecture structure:

```text
SkillHub
│
├── src
│   │
│   ├── SkillHub.Api
│   ├── SkillHub.Application
│   ├── SkillHub.Domain
│   └── SkillHub.Infrastructure
│
└── README.md
```

### Domain

The Domain layer contains the core business concepts and rules of the application.

The current Domain layer contains:

- `BaseEntity`
- `Participant`
- `Instructor`
- `Workshop`
- `Enrollment`
- `EnrollmentStatus`
- `UserRole`

### Current Domain Model

The main relationships are:

```text
Instructor
    │
    │ 1
    │
    │ *
    ▼
Workshop
    │
    │ 1
    │
    │ *
    ▼
Enrollment
    ▲
    │ *
    │
    │ 1
    │
Participant
```

A participant can have multiple enrollments.

A workshop can have multiple enrollments.

An instructor can teach multiple workshops.

`Enrollment` connects participants with workshops and stores information about the enrollment status and cancellation.

## Current Progress

### Domain Layer

The Domain layer has been completed with the following components:

- Base entity with a `Guid` identifier.
- Participant entity.
- Instructor entity.
- Workshop entity.
- Enrollment entity.
- Enrollment status enumeration.
- User role enumeration.
- Basic domain validations for required data.
- Workshop date validations.
- Enrollment cancellation behavior.

The Domain layer does not depend on Entity Framework Core, PostgreSQL, ASP.NET Core Identity, JWT, Excel libraries, or PDF libraries.

## Installation

### Prerequisites

Before running the project, make sure the following tools are installed:

- .NET 10 SDK
- Git
- JetBrains Rider or another compatible .NET IDE

### Clone the repository

Clone the repository using:

```bash
git clone https://github.com/harold-855/SkillHub.git
```

Move into the project directory:

```bash
cd SkillHub
```

### Restore the dependencies

Run:

```bash
dotnet restore
```

### Build the solution

Run:

```bash
dotnet build
```

## Running the Application

The API project can be started from JetBrains Rider by selecting `SkillHub.Api` as the startup project and running the application.

It can also be started from the terminal using:

```bash
dotnet run --project src/SkillHub.Api
```

> Database configuration, Identity, JWT authentication, initial data, Excel import, PDF generation, and frontend configuration will be added to this README as the corresponding features are implemented.
