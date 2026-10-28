# SkillHub

## Description
SkillHub is a training center platform designed to manage workshops and participant enrollments.

The application is developed as a complete solution composed of an **ASP.NET Core REST API** following Clean Architecture principles and a **decoupled Angular frontend** application. 

Participants can register, log in, browse available future workshops, view workshop and instructor details, enroll in workshops, view their active and cancelled enrollments, cancel eligible enrollments (with at least 24 hours anticipation), and download official PDF enrollment certificates.

Center employees can create workshops, review workshop enrollments, import enrollments from Excel files, and access analytical reports showing the most popular workshops directly via Swagger, Scalar, or API client tools.

The project is developed as part of the .NET Performance Assessment.

---

## Technologies
- **C# / .NET 10**
- **ASP.NET Core Web API**
- **Clean Architecture**
- **Entity Framework Core 10**
- **PostgreSQL** & **Npgsql** EF Core provider
- **ASP.NET Core Identity** (User and role authentication & authorization)
- **JWT (JSON Web Tokens)** Bearer Authentication
- **EPPlus** (Excel file reading and batch import)
- **QuestPDF** (High-quality PDF certificate generation)
- **Angular** (Decoupled SPA, Standalone Components, Angular Signals, Reactive Forms, HTTP Interceptors, Guards)
- **TypeScript & CSS3**

---

## Architecture
The solution follows Clean Architecture principles to enforce domain independence, testability, and separation of concerns.

```
SkillHub
├── src
│   ├── SkillHub.Api                    # Web API: controllers, middleware, CORS, and program entry
│   │   ├── Controllers
│   │   ├── Middleware
│   │   └── Program.cs
│   ├── SkillHub.Application            # Use cases, DTOs, repository interfaces, and service contracts
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   └── UseCases
│   ├── SkillHub.Domain                 # Business domain entities, value objects, and domain rules
│   │   ├── Entities
│   │   ├── Enums
│   │   └── Shared
│   └── SkillHub.Infrastructure         # PostgreSQL persistence, Identity, Excel import, and PDF generation
│       ├── Data
│       ├── Identity
│       ├── Imports
│       ├── Persistence
│       ├── Seeding
│       └── Services
├── frontend                            # Decoupled Angular Single-Page Application (SPA)
│   ├── src
│   │   ├── app
│   │   │   ├── core                    # Core models, services, interceptors, and guards
│   │   │   │   ├── guards              # Route guards (auth.guard.ts)
│   │   │   │   ├── interceptors        # HTTP interceptors (jwt.interceptor.ts)
│   │   │   │   ├── models              # TypeScript models and interfaces
│   │   │   │   └── services            # API service clients (auth, workshop, enrollment)
│   │   │   ├── features                # Feature-based presentation components
│   │   │   │   ├── auth                # Login & Registration views
│   │   │   │   ├── enrollments         # My Enrollments & certificate downloads
│   │   │   │   └── workshops           # Public catalog & workshop details
│   │   │   └── shared                  # Shared layout components (navbar)
│   │   ├── environments                # API endpoint configurations
│   │   ├── index.html
│   │   ├── main.ts
│   │   └── styles.css                  # Global responsive design system
│   ├── package.json
│   └── tsconfig.json
├── database
│   └── SkillHub_Database.sql           # Database schema and data export
└── README.md
```

### Dependency Direction
- **SkillHub.Domain** has no dependencies on other layers, databases, or frameworks.
- **SkillHub.Application** depends solely on `Domain` and defines use cases and contracts.
- **SkillHub.Infrastructure** implements persistence, identity, Excel parsing, and PDF rendering.
- **SkillHub.Api** configures dependency injection, authentication, authorization, CORS, and HTTP controllers.
- **Frontend** is completely decoupled and consumes the REST API through HTTP endpoints using JWT Bearer authentication.

---

## Domain Model & Rules
```
Instructor 1 ───── * Workshop 1 ───── * Enrollment * ───── 1 Participant
```
1. **Capacity Limit**: An enrollment cannot exceed the maximum capacity of a workshop; only active enrollments count towards this limit.
2. **No Duplicates**: A participant cannot have two active enrollments in the same workshop.
3. **No Schedule Conflicts**: A participant cannot enroll in a workshop that overlaps in schedule with another workshop where they already have an active enrollment.
4. **Future Workshops Only**: Participants can only enroll in future workshops that have not yet started.
5. **Cancellation Deadline**:
   - Participants can cancel their enrollment up to **24 hours** prior to the workshop start time.
   - Cancelling releases the seat immediately.
   - Center employees can cancel any participant's enrollment without the 24-hour restriction, provided the workshop has not started yet.
6. **Re-enrollment**: A participant who cancelled can enroll again if seats remain available.
7. **Certificate Generation**:
   - Certificates in PDF format are generated exclusively for **active** enrollments.
   - Cancelled enrollments are not eligible for certificates.
   - Participants can download their own certificates; employees can download any certificate.

---

## API Endpoints (`/api/v1`)

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Public | Register a new participant |
| `POST` | `/api/v1/auth/login` | Public | Authenticate and obtain JWT token |
| `GET` | `/api/v1/workshops` | Public | List available future workshops with calculated seats |
| `GET` | `/api/v1/workshops/{id}` | Public | Get detailed workshop & instructor information |
| `POST` | `/api/v1/workshops` | Employee | Create a new workshop |
| `GET` | `/api/v1/workshops/{id}/enrollments` | Employee | List all enrolled participants for a workshop |
| `POST` | `/api/v1/enrollments` | Participant | Enroll the authenticated participant in a workshop |
| `GET` | `/api/v1/enrollments/mine` | Participant | List enrollments for the authenticated participant |
| `PATCH`| `/api/v1/enrollments/{id}/cancel` | Owner / Employee | Cancel an eligible enrollment |
| `POST` | `/api/v1/enrollments/import` | Employee | Bulk import enrollments from Excel file |
| `GET` | `/api/v1/enrollments/{id}/certificate` | Owner / Employee | Download enrollment PDF certificate |
| `GET` | `/api/v1/reports/top-workshops` | Employee | Report of workshops ordered by active enrollment count |

---

## Decoupled Frontend (Angular)
The client application is built with modern Angular (Standalone Components, Signals, and Typed Forms).

### Key Features
1. **Public Catalog (`/workshops`)**:
   - Open to all visitors without requiring authentication.
   - Real-time search filter by title, workshop code, or instructor name.
   - Calculated available seat counters with dynamic color-coded badges.
2. **Workshop Detail (`/workshops/:id`)**:
   - Comprehensive information including instructor name, specialty, email, schedule, and remaining capacity.
   - Direct enrollment button with friendly login prompts for visitors.
3. **Authentication (`/login`, `/register`)**:
   - Reactive forms with client-side validation (matching passwords, required fields).
   - Problem Details (RFC 7807) error handling displaying domain conflicts clearly.
   - JWT token storage in `localStorage` and reactive state synchronization with Angular Signals.
4. **My Enrollments (`/my-enrollments`)**:
   - Protected by `authGuard`.
   - Lists active and cancelled enrollments with distinct visual indicators.
   - **Download Certificate (PDF)**: Directly downloads the binary PDF stream (`SkillHub-Constancia-<code>.pdf`) for active enrollments.
   - **Cancel Enrollment**: Checks and enforces the 24-hour cancellation rule with confirmation dialogs.
5. **CORS Support**:
   - Configured in the backend `Program.cs` (`AllowFrontend` policy) allowing seamless decoupled development from `http://localhost:4200`.

---

## Installation & Execution

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/) (running locally or in a container)
- [Node.js](https://nodejs.org/) (v20+ or v22+) & `npm`
- Git

### 1. Clone Repository
```bash
git clone https://github.com/harold-855/SkillHub.git
cd SkillHub
```

### 2. Configure Database & Secrets
Set your connection string in `src/SkillHub.Api/appsettings.json` or `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SkillHubDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "SecretKey": "YOUR_SUPER_SECRET_KEY_WITH_AT_LEAST_32_BYTES_LONG",
    "Issuer": "SkillHubApi",
    "Audience": "SkillHubApp"
  }
}
```

### 3. Apply Migrations & Seed Data
```bash
dotnet ef database update --project src/SkillHub.Infrastructure --startup-project src/SkillHub.Api
```
*The database seeder automatically loads initial instructors, participants, and workshops from `SkillHub_Data.xlsx` upon first launch.*

### 4. Run Backend API
```bash
dotnet run --project src/SkillHub.Api
```
- API HTTP: `http://localhost:5109`
- API OpenAPI / Swagger: `http://localhost:5109/openapi/v1.json`

### 5. Run Angular Frontend
In a separate terminal window:
```bash
cd frontend
npm install
npm start
```
Open your browser and navigate to:
**[http://localhost:4200](http://localhost:4200)**

---

## Initial Employee Account
Employees are seeded automatically at application startup from the seed spreadsheet. Use these credentials to test employee-only endpoints:
- **Email**: `REPLACE_WITH_SEEDED_EMPLOYEE_EMAIL`
- **Password**: `REPLACE_WITH_SEEDED_EMPLOYEE_PASSWORD`

---

## Database Export
The assessment deliverable requires an exported SQL script containing both schema and data:
- **Location**: `database/SkillHub_Database.sql`

---

## Implementation Status
- [x] Domain entities, enums, and domain validation rules.
- [x] Application DTOs, use cases, and repository interfaces.
- [x] PostgreSQL persistence with Entity Framework Core Code First and migrations.
- [x] ASP.NET Core Identity authentication and role management (Participant & Employee).
- [x] JWT Bearer token generation and authorization policies.
- [x] Initial data seeding from `SkillHub_Data.xlsx`.
- [x] RESTful controllers and Problem Details exception handling middleware.
- [x] Excel batch enrollment import (`POST /api/v1/enrollments/import`).
- [x] PDF certificate generation using QuestPDF (`GET /api/v1/enrollments/{id}/certificate`).
- [x] CORS middleware enabling decoupled client access.
- [x] **Decoupled Angular frontend** covering full participant lifecycle:
  - Registration & Login
  - Public Workshop Catalog & Details
  - Workshop Enrollment
  - Personal Enrollments Management
  - 24-hour Cancellation Rule Enforcement
  - PDF Certificate Download
- [ ] Export database schema and data to `database/SkillHub_Database.sql`.
- [ ] Fill employee credentials and author details before final submission.

---

## Author
- **Full Name**: REPLACE_WITH_YOUR_FULL_NAME
- **Clan**: REPLACE_WITH_YOUR_CLAN
- **Email**: REPLACE_WITH_YOUR_EMAIL
- **Document Number**: REPLACE_WITH_YOUR_DOCUMENT_NUMBER
