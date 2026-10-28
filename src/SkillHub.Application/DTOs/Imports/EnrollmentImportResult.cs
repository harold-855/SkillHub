namespace SkillHub.Application.DTOs.Imports;

/// <summary>
/// Summary report returned upon completion of an Excel batch enrollment import.
/// </summary>
public class EnrollmentImportResult
{
    /// <summary>The total count of rows processed from the Excel sheet.</summary>
    public int TotalRows { get; set; }

    /// <summary>The total number of enrollments successfully created.</summary>
    public int SuccessfulEnrollments { get; set; }

    /// <summary>The total number of rows rejected due to business validation failures.</summary>
    public int RejectedRows { get; set; }

    /// <summary>Detailed list of validation failure reasons for each rejected row.</summary>
    public List<EnrollmentImportError> Errors { get; set; } = new();
}

/// <summary>
/// Information detailing why an individual Excel row failed enrollment.
/// </summary>
public class EnrollmentImportError
{
    /// <summary>The 1-based spreadsheet row number.</summary>
    public int RowNumber { get; set; }

    /// <summary>The participant email from the row.</summary>
    public string ParticipantEmail { get; set; } = string.Empty;

    /// <summary>The workshop code from the row.</summary>
    public string WorkshopCode { get; set; } = string.Empty;

    /// <summary>The detailed domain reason why this row could not be enrolled.</summary>
    public string Reason { get; set; } = string.Empty;
}
