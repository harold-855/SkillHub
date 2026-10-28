SkillHub
Description
SkillHub is a training center platform designed to manage workshops and participant enrollments.

The application is developed as a .NET solution following Clean Architecture principles. Participants can register, log in, browse available workshops, enroll in workshops, view their enrollments, cancel eligible enrollments, and download enrollment certificates.

Employees can create workshops, review workshop enrollments, import enrollments from Excel files, and access reports showing the most popular workshops.

The project is being developed as part of a .NET performance assessment.
Technologies
- C#
- .NET 10
- ASP.NET Core Web API
- Clean Architecture
- Entity Framework Core 10
- PostgreSQL
- Npgsql — PostgreSQL provider for Entity Framework Core
- ASP.NET Core Identity — user and role management
- JWT (JSON Web Tokens) — API authentication
- EPPlus — Excel file reading
- QuestPDF — PDF certificate generation
- Angular — planned decoupled participant-facing frontend
- JetBrains Rider — development environment
  Architecture
  The solution follows Clean Architecture principles. The Domain contains business entities and rules; Application contains use cases and abstractions; Infrastructure implements persistence and external-service integrations; and the API exposes HTTP endpoints.
  SkillHub
  ├── src
  │   ├── SkillHub.Api
  │   │   ├── Controllers
  │   │   ├── Middleware
  │   │   └── Program.cs
  │   ├── SkillHub.Application
  │   │   ├── DTOs
  │   │   ├── Interfaces
  │   │   ├── Services
  │   │   └── UseCases
  │   ├── SkillHub.Domain
  │   │   ├── Entities
  │   │   ├── Enums
  │   │   └── Shared
  │   └── SkillHub.Infrastructure
  │       ├── Data
  │       │   └── Seed
  │       ├── Identity
  │       ├── Imports
  │       │   └── Excel
  │       ├── Persistence
  │       │   ├── Configurations
  │       │   ├── Migrations
  │       │   └── Repositories
  │       ├── Seeding
  │       └── Services
  │           └── Pdf
  ├── database
  │   └── SkillHub_Database.sql
  └── README.md
  The database/SkillHub_Database.sql file should be included after exporting the database schema and data from PostgreSQL. If the export has not yet been placed there, this path describes the intended location.
  Dependency Direction
- SkillHub.Domain has no dependency on the other application layers or infrastructure technologies.
- SkillHub.Application references SkillHub.Domain and defines use cases and interfaces.
- SkillHub.Infrastructure references Application and Domain to implement persistence and external integrations.
- SkillHub.Api references Application and Infrastructure, configures dependency injection, authentication, authorization, middleware, and HTTP endpoints.
  Application does not need to know how a database or PDF/Excel library works. It uses interfaces; Infrastructure provides their implementations.
  Domain Layer
  The Domain layer contains the core business concepts and rules.

Current components:
- BaseEntity
- Participant
- Instructor
- Workshop
- Enrollment
- EnrollmentStatus
- UserRole
  Domain Model
  Instructor 1 ───── * Workshop 1 ───── * Enrollment * ───── 1 Participant
- An instructor can teach multiple workshops.
- A workshop can have multiple enrollments.
- A participant can have multiple enrollments.
- Enrollment connects a participant with a workshop and stores its status and cancellation information.
  The entities include basic validations for required data, workshop dates, capacity, and enrollment cancellation. The Domain layer does not depend on Entity Framework Core, PostgreSQL, ASP.NET Core Identity, JWT, Excel libraries, or PDF libraries.
  Application Layer
  The Application layer coordinates the system's operations. It contains DTOs, repository contracts, service abstractions, and use cases. It references Domain and does not directly depend on the database implementation.
  DTOs
  DTOs transfer data between the API and application operations without exposing domain entities directly. They cover authentication, participants, workshops, enrollments, imports, certificates, and reports.
  Repository Interfaces
- IParticipantRepository: retrieves and creates participants and checks whether an email exists.
- IInstructorRepository: retrieves instructors and checks instructor email existence.
- IWorkshopRepository: retrieves workshops, gets the public catalog, creates workshops, and checks workshop codes.
- IEnrollmentRepository: retrieves, creates, and updates enrollments; checks active duplicates and schedule conflicts; and counts active enrollments.
  Application Service Interfaces
- IAuthenticationService
- IJwtTokenGenerator
- IEnrollmentExcelReader
- ICertificateGenerator
  These abstractions allow use cases to request operations without depending on specific technical implementations.
  Implemented Use Cases
  Authentication
- RegisterParticipantUseCase
- LoginUseCase
  Workshop management
- CreateWorkshopUseCase
- GetAvailableWorkshopsUseCase
- GetWorkshopByIdUseCase
- GetTopWorkshopsUseCase
  Enrollment management
- CreateEnrollmentUseCase
- GetMyEnrollmentsUseCase
- GetWorkshopEnrollmentsUseCase
- CancelEnrollmentUseCase
  Excel import
- ImportEnrollmentsUseCase
  Certificates
- GenerateEnrollmentCertificateUseCase
  The enrollment use cases check rules such as workshop start time, active duplicate enrollments, capacity, and schedule conflicts. Cancellation rules and other business constraints should be verified against the assessment requirements during final review.
  Infrastructure Layer
  The Infrastructure layer provides technical implementations for persistence, identity, Excel reading, and PDF generation.
  PostgreSQL and Entity Framework Core
- SkillHubDbContext extends IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>.
- The context exposes sets for participants, instructors, workshops, and enrollments.
- Entity configurations are loaded through ApplyConfigurationsFromAssembly.
- Repository implementations provide data access for the Application layer.
- Entity Framework Core migrations are included in the project, including InitialCreate and AddIdentityTables.
- PostgreSQL is accessed through the Npgsql EF Core provider.
  Identity and JWT
  ASP.NET Core Identity manages user accounts and roles. The application configures the Participant and Employee roles and uses JWT bearer authentication to protect endpoints. Employee-only operations are restricted by role-based authorization.

The initial data seeding process creates the employee accounts and assigns roles. Use the credentials configured for the project when demonstrating employee endpoints; do not publish real production passwords in a public repository.
Initial Data
InitialDataSeeder loads instructors, participants, and workshops from the workbook located at:

src/SkillHub.Infrastructure/Data/Seed/SkillHub_Data.xlsx

The seeder also creates the corresponding Identity users and assigns roles. Enrollments are intentionally not created directly by the seeder; they are created through the enrollment workflow or Excel import endpoint.

The workbook's workshop dates must satisfy the application's date rules when the seed process runs.
Excel Enrollment Import
EpplusEnrollmentExcelReader reads the Enrollments worksheet and expects the columns ParticipantEmail and WorkshopCode.

The import use case processes rows through the normal enrollment use case, so imported records are subject to the same enrollment validations. The result includes totals, successful enrollments, and rejected rows with reasons.
PDF Certificates
PdfCertificateGenerator uses QuestPDF to generate enrollment certificates. The Application layer requests generation through ICertificateGenerator, keeping the PDF library out of the Application and Domain layers.

The certificate includes SkillHub branding, the title “Constancia de inscripción”, participant details, workshop details, instructor, workshop dates/times, enrollment date, and generation date.
API Endpoints
The API uses versioned routes under /api/v1.
Method	Endpoint	Access	Purpose
POST	/api/v1/auth/register	Public	Register a participant
POST	/api/v1/auth/login	Public	Authenticate and obtain a JWT
GET	/api/v1/workshops	Public	List workshops available in the public catalog
GET	/api/v1/workshops/{id}	Public	Get workshop details
POST	/api/v1/workshops	Employee	Create a workshop
GET	/api/v1/workshops/{id}/enrollments	Employee	List enrollments for a workshop
POST	/api/v1/enrollments	Participant	Enroll in a workshop
GET	/api/v1/enrollments/mine	Participant	List the current participant's enrollments
PATCH	/api/v1/enrollments/{id}/cancel	Owner or Employee	Cancel an enrollment when permitted
POST	/api/v1/enrollments/import	Employee	Import enrollments from an Excel file
GET	/api/v1/enrollments/{id}/certificate	Owner or Employee	Download an enrollment certificate as PDF
GET	/api/v1/reports/top-workshops	Employee	List workshops ordered by active enrollment count


Authentication and authorization requirements are enforced by the API. Exact request/response payloads and all business-rule outcomes should be checked during the final API review.
Error Handling
The API includes exception-handling middleware that returns structured Problem Details responses. The current mappings include:
- ArgumentException → 400 Bad Request
- UnauthorizedAccessException → 401 Unauthorized
- KeyNotFoundException → 404 Not Found
- InvalidOperationException → 409 Conflict
- Other unhandled exceptions → 500 Internal Server Error
  These mappings should be verified against the expected HTTP semantics during final review.
  Installation and Configuration
  Prerequisites
- .NET 10 SDK
- PostgreSQL
- Git
- JetBrains Rider or another compatible .NET IDE
  Clone the Repository
  git clone https://github.com/harold-855/SkillHub.git
  cd SkillHub
  Configure the Database Connection
  Configure the ConnectionStrings:DefaultConnection setting for your local PostgreSQL database. Keep local passwords and other secrets out of source control. Use local configuration, user secrets, or environment variables as appropriate.

The API also requires JWT settings under Jwt, including:
- SecretKey — a secret key with at least 32 bytes
- Issuer
- Audience
  Use your own local values. Never commit real production secrets.
  Restore and Build
  dotnet restore
  dotnet build
  Database Migrations
  The Infrastructure project contains Entity Framework Core migrations. Ensure the connection string is configured before applying migrations. From the solution root, a typical command is:
  dotnet ef database update --project src/SkillHub.Infrastructure --startup-project src/SkillHub.Api
  If the EF Core CLI tool is not installed, install it according to the official .NET tooling instructions. The command assumes the project paths match the repository structure above.
  Run the API
  From JetBrains Rider, select SkillHub.Api as the startup project and run it.

Alternatively:
dotnet run --project src/SkillHub.Api
In development, the API exposes its OpenAPI document through the configured OpenAPI endpoint. Use the actual URL printed by the application when it starts.
Initial Employee Accounts
Employee accounts are created by the initial seeding process. Enter the actual seeded employee credentials here before submission:
- Email: REPLACE_WITH_SEEDED_EMPLOYEE_EMAIL
- Password: REPLACE_WITH_SEEDED_EMPLOYEE_PASSWORD
  Replace these placeholders with the correct assessment/demo credentials. Do not commit any real or reused password that should remain secret.
  Database Export
  The assessment requires a SQL script containing both the database schema and data.

Export the SkillHub PostgreSQL database as a plain SQL file and place it at:

database/SkillHub_Database.sql

The export should include table definitions and data, not only the schema or only the data. Check the file before submission to confirm it contains SQL statements for creating the database objects and restoring the records.
Frontend
The participant-facing Angular frontend is planned as a separate application, not as a monolithic part of the ASP.NET Core API. It should support participant registration and login, browsing workshops and available seats, enrolling, viewing personal enrollments, cancelling eligible enrollments, and downloading PDF certificates.

Status: Angular frontend implementation and end-to-end integration remain to be completed.
Current Implementation Status
The following backend components have been created:
- Domain entities, enums, and basic business validations.
- Application DTOs, repository interfaces, service abstractions, and use cases.
- PostgreSQL persistence with EF Core, entity configurations, repositories, and migrations.
- ASP.NET Core Identity roles and JWT authentication configuration.
- Initial data seeding from SkillHub_Data.xlsx.
- API controllers for authentication, workshops, enrollments, and reports.
- Excel enrollment reader and import endpoint.
- PDF certificate generation through QuestPDF.
- Employee authorization for employee-only endpoints.
- Exception-handling middleware.
  Implementation does not by itself guarantee that every assessment rule has been fully verified. The following tasks remain before delivery:
- Complete and integrate the Angular frontend.
- Confirm the SQL export contains both schema and data and is stored in the repository.
- Replace the employee credential placeholders with the intended demo credentials.
- Complete the README author details.
- Perform the final review of endpoint behavior, authorization, cancellation deadlines, partial Excel-import persistence, seed-data dates, and other assessment rules.
- Run final end-to-end checks.
- Create the final ZIP deliverable and confirm the public GitHub repository is ready for submission.
  Author
  Complete this section before submission:
- Full name: REPLACE_WITH_YOUR_FULL_NAME
- Clan: REPLACE_WITH_YOUR_CLAN
- Email: REPLACE_WITH_YOUR_EMAIL
- Document number: REPLACE_WITH_YOUR_DOCUMENT_NUMBER