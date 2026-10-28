using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using SkillHub.Api.Middleware;
using SkillHub.Application.Interfaces.Authentication;
using SkillHub.Application.Interfaces.Documents;
using SkillHub.Application.Interfaces.Imports;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.UseCases.Authentication.Login;
using SkillHub.Application.UseCases.Authentication.RegisterParticipant;
using SkillHub.Application.UseCases.Enrollments.CancelEnrollment;
using SkillHub.Application.UseCases.Enrollments.CreateEnrollment;
using SkillHub.Application.UseCases.Enrollments.GenerateEnrollmentCertificate;
using SkillHub.Application.UseCases.Enrollments.GetMyEnrollments;
using SkillHub.Application.UseCases.Enrollments.ImportEnrollments;
using SkillHub.Application.UseCases.Reports.GetTopWorkshops;
using SkillHub.Application.UseCases.Workshops.CreateWorkshop;
using SkillHub.Application.UseCases.Workshops.GetAvailableWorkshops;
using SkillHub.Application.UseCases.Workshops.GetWorkshopById;
using SkillHub.Application.UseCases.Workshops.GetWorkshopEnrollments;
using SkillHub.Infrastructure.Identity;
using SkillHub.Infrastructure.Imports.Excel;
using SkillHub.Infrastructure.Persistence;
using SkillHub.Infrastructure.Persistence.Repositories;
using SkillHub.Infrastructure.Seeding;
using SkillHub.Infrastructure.Services.Pdf;

// Configure QuestPDF license.
// Use Community only if your usage meets its license conditions.
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Register API services.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Configure CORS for decoupled frontend applications.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register authentication use cases.
builder.Services.AddScoped<RegisterParticipantUseCase>();
builder.Services.AddScoped<LoginUseCase>();

// Register workshop use cases.
builder.Services.AddScoped<CreateWorkshopUseCase>();
builder.Services.AddScoped<GetAvailableWorkshopsUseCase>();
builder.Services.AddScoped<GetWorkshopByIdUseCase>();
builder.Services.AddScoped<GetWorkshopEnrollmentsUseCase>();

// Register enrollment use cases.
builder.Services.AddScoped<CreateEnrollmentUseCase>();
builder.Services.AddScoped<GetMyEnrollmentsUseCase>();
builder.Services.AddScoped<CancelEnrollmentUseCase>();
builder.Services.AddScoped<ImportEnrollmentsUseCase>();
builder.Services.AddScoped<GenerateEnrollmentCertificateUseCase>();

// Register report use cases.
builder.Services.AddScoped<GetTopWorkshopsUseCase>();

// Configure PostgreSQL database connection.
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The database connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<SkillHubDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure ASP.NET Core Identity.
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<SkillHubDbContext>();

// Configure JWT authentication.
var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException(
        "The JWT secret key is not configured.");

var issuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException(
        "The JWT issuer is not configured.");

var audience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException(
        "The JWT audience is not configured.");

var key = Encoding.UTF8.GetBytes(secretKey);

if (key.Length < 32)
{
    throw new InvalidOperationException(
        "The JWT secret key must contain at least 32 bytes.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ClockSkew = TimeSpan.Zero
        };
    });

// Configure authorization.
builder.Services.AddAuthorization();

// Register authentication services.
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Register repositories.
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<IWorkshopRepository, WorkshopRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// Register Excel reader.
builder.Services.AddScoped<
    IEnrollmentExcelReader,
    EpplusEnrollmentExcelReader>();

// Register PDF certificate generator.
builder.Services.AddScoped<
    ICertificateGenerator,
    PdfCertificateGenerator>();

// Register seeders.
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddScoped<InitialDataSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Initialize Identity roles and initial application data.
using (var scope = app.Services.CreateScope())
{
    var identitySeeder = scope.ServiceProvider
        .GetRequiredService<IdentitySeeder>();

    await identitySeeder.SeedRolesAsync();

    var initialDataSeeder = scope.ServiceProvider
        .GetRequiredService<InitialDataSeeder>();

    var excelFilePath = Path.GetFullPath(
        Path.Combine(
            builder.Environment.ContentRootPath,
            "..",
            "SkillHub.Infrastructure",
            "Data",
            "Seed",
            "SkillHub_Data.xlsx"));

    await initialDataSeeder.SeedAsync(excelFilePath);
}

// Configure endpoints and remaining middleware.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS middleware for decoupled client requests before authentication & authorization.
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints.
app.MapControllers();

app.Run();


