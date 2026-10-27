namespace SkillHub.Application.DTOs.Imports;

public class EnrollmentImportResult
{
    public int TotalRows { get; set; }

    public int SuccessfulEnrollments { get; set; }

    public int RejectedRows { get; set; }

    public List<EnrollmentImportError> Errors { get; set; } = new();
}

public class EnrollmentImportError
{
    public int RowNumber { get; set; }

    public string ParticipantEmail { get; set; } = string.Empty;

    public string WorkshopCode { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}