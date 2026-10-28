using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SkillHub.Domain.Entities;
using SkillHub.Infrastructure.Identity;
using SkillHub.Infrastructure.Persistence;

namespace SkillHub.Infrastructure.Seeding;

/// <summary>
/// Data seeder reading initial instructors, participants, and workshops from the provided Excel workbook
/// and populating both domain entities and ASP.NET Core Identity user accounts.
/// </summary>
public class InitialDataSeeder
{
    private readonly SkillHubDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitialDataSeeder"/> class.
    /// </summary>
    public InitialDataSeeder(
        SkillHubDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    /// <summary>
    /// Reads and seeds instructors, participants, and workshops from the designated Excel file.
    /// </summary>
    /// <param name="filePath">The absolute path to SkillHub_Data.xlsx.</param>
    public async Task SeedAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "The initial data Excel file was not found.",
                filePath);
        }

        ExcelPackage.License.SetNonCommercialPersonal(
            "Harold Aristizabal");

        using var package = new ExcelPackage(new FileInfo(filePath));

        await SeedInstructorsAsync(package);
        await SeedParticipantsAsync(package);
        await SeedWorkshopsAsync(package);
    }

    private async Task SeedInstructorsAsync(ExcelPackage package)
    {
        var worksheet = package.Workbook.Worksheets["Instructors"];

        if (worksheet?.Dimension is null)
        {
            throw new InvalidOperationException(
                "The 'Instructors' worksheet is missing or empty.");
        }

        var columns = GetColumns(worksheet);

        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var fullName = GetText(worksheet, row, columns, "FullName");
            var email = GetText(worksheet, row, columns, "Email")
                .ToLowerInvariant();
            var password = GetText(worksheet, row, columns, "Password");
            var specialty = GetText(worksheet, row, columns, "Specialty");

            if (string.IsNullOrWhiteSpace(email))
            {
                continue;
            }

            var instructor = await _dbContext.Instructors
                .FirstOrDefaultAsync(x => x.Email == email);

            var user = await _userManager.FindByEmailAsync(email);

            if (instructor is null &&
                string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidOperationException(
                    $"The instructor's name is missing in row {row}.");
            }

            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        $"The instructor's password is missing in row {row}.");
                }

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);

                EnsureIdentitySuccess(
                    result,
                    $"creating the instructor account '{email}'");
            }

            await EnsureRoleAsync(user, "Employee");

            if (instructor is null)
            {
                instructor = new Instructor(
                    fullName,
                    email,
                    specialty);

                _dbContext.Instructors.Add(instructor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedParticipantsAsync(ExcelPackage package)
    {
        var worksheet = package.Workbook.Worksheets["Participants"];

        if (worksheet?.Dimension is null)
        {
            throw new InvalidOperationException(
                "The 'Participants' worksheet is missing or empty.");
        }

        var columns = GetColumns(worksheet);

        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var fullName = GetText(worksheet, row, columns, "FullName");
            var email = GetText(worksheet, row, columns, "Email")
                .ToLowerInvariant();
            var password = GetText(worksheet, row, columns, "Password");

            if (string.IsNullOrWhiteSpace(email))
            {
                continue;
            }

            var participant = await _dbContext.Participants
                .FirstOrDefaultAsync(x => x.Email == email);

            var user = await _userManager.FindByEmailAsync(email);

            if (participant is null &&
                string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidOperationException(
                    $"The participant's name is missing in row {row}.");
            }

            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        $"The participant's password is missing in row {row}.");
                }

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);

                EnsureIdentitySuccess(
                    result,
                    $"creating the participant account '{email}'");
            }

            await EnsureRoleAsync(user, "Participant");

            if (participant is null)
            {
                participant = new Participant(fullName, email);

                _dbContext.Participants.Add(participant);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedWorkshopsAsync(ExcelPackage package)
    {
        var worksheet = package.Workbook.Worksheets["Workshops"];

        if (worksheet?.Dimension is null)
        {
            throw new InvalidOperationException(
                "The 'Workshops' worksheet is missing or empty.");
        }

        var columns = GetColumns(worksheet);

        var colombiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var code = GetText(worksheet, row, columns, "Code")
                .ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(code))
            {
                continue;
            }

            var existingWorkshop = await _dbContext.Workshops
                .AnyAsync(x => x.Code == code);

            if (existingWorkshop)
            {
                continue;
            }

            var title = GetText(worksheet, row, columns, "Title");
            var description = GetText(
                worksheet, row, columns, "Description");

            var instructorEmail = GetText(
                worksheet, row, columns, "InstructorEmail")
                .ToLowerInvariant();

            var instructor = await _dbContext.Instructors
                .FirstOrDefaultAsync(x => x.Email == instructorEmail);

            if (instructor is null)
            {
                throw new InvalidOperationException(
                    $"The instructor '{instructorEmail}' for workshop " +
                    $"'{code}' was not found.");
            }

            var startLocal = GetDateTime(
                worksheet, row, columns, "StartDateTime");

            var endLocal = GetDateTime(
                worksheet, row, columns, "EndDateTime");

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(startLocal, DateTimeKind.Unspecified),
                colombiaTimeZone);

            var endUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(endLocal, DateTimeKind.Unspecified),
                colombiaTimeZone);

            var capacity = GetInteger(
                worksheet, row, columns, "Capacity");

            var workshop = new Workshop(
                code,
                title,
                description,
                instructor.Id,
                startUtc,
                endUtc,
                capacity);

            _dbContext.Workshops.Add(workshop);

            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task EnsureRoleAsync(
        ApplicationUser user,
        string role)
    {
        if (await _userManager.IsInRoleAsync(user, role))
        {
            return;
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        EnsureIdentitySuccess(
            result,
            $"assigning the role '{role}' to '{user.Email}'");
    }

    private static Dictionary<string, int> GetColumns(
        OfficeOpenXml.ExcelWorksheet worksheet)
    {
        var columns = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        for (var column = 1;
             column <= worksheet.Dimension!.Columns;
             column++)
        {
            var header = worksheet.Cells[1, column].Text.Trim();

            if (!string.IsNullOrWhiteSpace(header))
            {
                columns[header] = column;
            }
        }

        return columns;
    }

    private static string GetText(
        OfficeOpenXml.ExcelWorksheet worksheet,
        int row,
        Dictionary<string, int> columns,
        string columnName)
    {
        if (!columns.TryGetValue(columnName, out var column))
        {
            throw new InvalidOperationException(
                $"The Excel column '{columnName}' was not found.");
        }

        return worksheet.Cells[row, column].Text.Trim();
    }

    private static DateTime GetDateTime(
        OfficeOpenXml.ExcelWorksheet worksheet,
        int row,
        Dictionary<string, int> columns,
        string columnName)
    {
        if (!columns.TryGetValue(columnName, out var column))
        {
            throw new InvalidOperationException(
                $"The Excel column '{columnName}' was not found.");
        }

        if (!DateTime.TryParse(
                worksheet.Cells[row, column].Text,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var dateTime))
        {
            throw new InvalidOperationException(
                $"The date in column '{columnName}', row {row}, is invalid.");
        }

        return dateTime;
    }

    private static int GetInteger(
        OfficeOpenXml.ExcelWorksheet worksheet,
        int row,
        Dictionary<string, int> columns,
        string columnName)
    {
        var value = GetText(worksheet, row, columns, columnName);

        if (!int.TryParse(value, out var number))
        {
            throw new InvalidOperationException(
                $"The value in column '{columnName}', row {row}, is invalid.");
        }

        return number;
    }

    private static void EnsureIdentitySuccess(
        IdentityResult result,
        string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(
            "; ",
            result.Errors.Select(error => error.Description));

        throw new InvalidOperationException(
            $"An error occurred while {operation}: {errors}");
    }
}
