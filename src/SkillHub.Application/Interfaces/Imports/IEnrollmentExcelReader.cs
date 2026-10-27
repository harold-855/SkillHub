namespace SkillHub.Application.Interfaces.Imports;

public interface IEnrollmentExcelReader
{
    Task<IReadOnlyCollection<EnrollmentImportRow>> ReadAsync(
        Stream fileStream);
}

public class EnrollmentImportRow
{
    public int RowNumber { get; set; }

    public string ParticipantEmail { get; set; } = string.Empty;

    public string WorkshopCode { get; set; } = string.Empty;
}   