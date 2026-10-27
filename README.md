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

## Application Layer

The Application layer contains the use cases that coordinate the system's operations. It defines the interfaces required to access data and external services without depending directly on infrastructure technologies.

This layer references the Domain layer and uses its entities and business rules to implement the application's functionality.

### Data Transfer Objects (DTOs)

DTOs are used to transfer information between application components without exposing domain entities directly.

The current DTOs are organized into the following categories:

- **Authentication:** Registration requests, login requests, and authentication responses.
- **Certificates:** Data required to generate enrollment certificates.
- **Enrollments:** Enrollment creation requests and enrollment responses.
- **Imports:** Results and error details for Excel enrollment imports.
- **Participants:** Participant information and participant summaries.
- **Workshops:** Workshop creation requests, workshop responses, workshop details, and top-workshop report data.

### Repository Interfaces

The Application layer defines repository interfaces that describe the data access operations required by the use cases.

The current interfaces are:

- `IParticipantRepository`: Defines operations to retrieve, create, and check the existence of participants.
- `IInstructorRepository`: Defines operations to retrieve instructors and check whether an instructor exists by email.
- `IWorkshopRepository`: Defines operations to retrieve workshops, obtain available workshops, retrieve all workshops, create workshops, and check workshop codes.
- `IEnrollmentRepository`: Defines operations to retrieve enrollments, create and update enrollments, check duplicate active enrollments, detect schedule conflicts, and count active enrollments.

These interfaces allow the Application layer to remain independent of the database implementation.

### Authentication

The authentication use cases currently include:

- `RegisterParticipantUseCase`: Validates participant registration data and delegates account registration to the authentication service.
- `LoginUseCase`: Validates login data and delegates authentication to the authentication service.

The authentication interface is designed to support a future implementation using ASP.NET Core Identity and JSON Web Tokens (JWT).

Identity integration, token generation, and authorization configuration are still pending.

### Workshop Management

The workshop use cases currently include:

- `CreateWorkshopUseCase`: Validates the instructor and workshop code before creating a workshop.
- `GetAvailableWorkshopsUseCase`: Retrieves workshops for the public catalog and calculates available seats.
- `GetWorkshopByIdUseCase`: Retrieves the details of a specific workshop.
- `GetTopWorkshopsUseCase`: Builds a report of workshops ordered by the number of active enrollments.

The repository interface distinguishes between retrieving available workshops and retrieving all workshops, allowing the public catalog and employee reports to use different queries.

### Enrollment Management

The enrollment use cases currently include:

- `CreateEnrollmentUseCase`: Validates participant existence, workshop availability, duplicate active enrollments, capacity, and schedule conflicts before creating an enrollment.
- `GetMyEnrollmentsUseCase`: Retrieves the enrollments associated with a participant.
- `GetWorkshopEnrollmentsUseCase`: Retrieves enrollment information for a specific workshop.
- `CancelEnrollmentUseCase`: Checks ownership, enrollment status, workshop start time, and the participant cancellation deadline before requesting that the enrollment changes be persisted.

The enrollment process is designed to prevent duplicate active enrollments, overbooking, and schedule conflicts.

The repository implementation and database persistence still need to be completed in the Infrastructure layer.

### Excel Enrollment Import

The application includes an initial implementation of `ImportEnrollmentsUseCase` and the following supporting components:

- `IEnrollmentExcelReader`: Defines the contract for reading enrollment data from an Excel file.
- `EnrollmentImportRow`: Represents an individual row containing a participant email and workshop code.
- `EnrollmentImportResult`: Represents the import summary, including the total number of rows, successful enrollments, rejected rows, and error details.
- `EnrollmentImportError`: Records the row number, participant email, workshop code, and rejection reason.

The import use case reuses `CreateEnrollmentUseCase` so that imported enrollments follow the same business validations as regular enrollments.

The Excel reader implementation, file upload endpoint, and complete import workflow are still pending.

### Enrollment Certificates

The application includes `GenerateEnrollmentCertificateUseCase`, which:

1. Retrieves the enrollment.
2. Checks that the enrollment is active.
3. Verifies whether the current user is authorized to download the certificate.
4. Retrieves the participant, workshop, and instructor information.
5. Builds an `EnrollmentCertificateData` object.
6. Requests PDF generation through `ICertificateGenerator`.

The `ICertificateGenerator` interface defines the contract for generating a certificate from the required data.

The actual PDF generation implementation will be developed in Infrastructure. The API endpoint and download workflow are also pending.

## Current Implementation Status

The Domain and Application projects have been created, and the solution has compiled successfully during development.

The current implementation includes the main domain entities, repository contracts, DTOs, and use cases for authentication, workshop management, enrollments, Excel import preparation, certificates, and reporting.

However, successful compilation does not mean that every feature is fully implemented or operational.

The following work remains:

- Implement Entity Framework Core persistence with PostgreSQL.
- Create the database context and entity mappings.
- Implement the repository interfaces in Infrastructure.
- Configure ASP.NET Core Identity and JWT authentication.
- Implement employee and participant authorization.
- Load the required initial data and accounts.
- Implement Excel file reading and the import endpoint.
- Implement PDF certificate generation.
- Create the REST API controllers and endpoints.
- Develop the decoupled Angular frontend.
- Generate and validate database migrations.
- Prepare the final SQL schema and data script.
- Complete testing and verify the required business rules.

## Next Development Stage

The next stage is the Infrastructure layer.

This layer will implement the interfaces defined in Application and provide the technical components required for database access, authentication integration, Excel processing, and PDF generation.

The objective is to preserve the Clean Architecture dependency direction while connecting the application's use cases to the required technologies.