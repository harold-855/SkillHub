namespace SkillHub.Application.Interfaces.Imports;

/// <summary>
/// Service abstraction for parsing batch workshop enrollment data from an Excel worksheet.
/// </summary>
public interface IEnrollmentExcelReader
{
    /// <summary>
    /// Reads and extracts enrollment rows from the provided Excel file stream.
    /// </summary>
    Task<IReadOnlyCollection<EnrollmentImportRow>> ReadAsync(
        Stream fileStream);
}

/// <summary>
/// Data transfer representation of a single row extracted from the Excel enrollment import spreadsheet.
/// </summary>
public class EnrollmentImportRow
{
    /// <summary>
    /// The physical 1-based row index in the spreadsheet.
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// The email address of the participant to be enrolled.
    /// </summary>
    public string ParticipantEmail { get; set; } = string.Empty;

    /// <summary>
    /// The unique alphanumeric code of the workshop.
    /// </summary>
    public string WorkshopCode { get; set; } = string.Empty;
}
