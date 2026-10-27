using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.DTOs.Imports;
using SkillHub.Application.Interfaces.Imports;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.UseCases.Enrollments.CreateEnrollment;

namespace SkillHub.Application.UseCases.Enrollments.ImportEnrollments;

public class ImportEnrollmentsUseCase
{
    private readonly IEnrollmentExcelReader _excelReader;
    private readonly IParticipantRepository _participantRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly CreateEnrollmentUseCase _createEnrollmentUseCase;

    public ImportEnrollmentsUseCase(
        IEnrollmentExcelReader excelReader,
        IParticipantRepository participantRepository,
        IWorkshopRepository workshopRepository,
        CreateEnrollmentUseCase createEnrollmentUseCase)
    {
        _excelReader = excelReader;
        _participantRepository = participantRepository;
        _workshopRepository = workshopRepository;
        _createEnrollmentUseCase = createEnrollmentUseCase;
    }

    public async Task<EnrollmentImportResult> ExecuteAsync(Stream fileStream)
    {
        var rows = await _excelReader.ReadAsync(fileStream);

        var result = new EnrollmentImportResult
        {
            TotalRows = rows.Count
        };

        foreach (var row in rows)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.ParticipantEmail))
                {
                    throw new InvalidOperationException(
                        "Participant email is required.");
                }

                if (string.IsNullOrWhiteSpace(row.WorkshopCode))
                {
                    throw new InvalidOperationException(
                        "Workshop code is required.");
                }

                var participant = await _participantRepository
                    .GetByEmailAsync(row.ParticipantEmail.Trim());

                if (participant is null)
                {
                    throw new InvalidOperationException(
                        "The participant does not exist.");
                }

                var workshop = await _workshopRepository
                    .GetByCodeAsync(row.WorkshopCode.Trim());

                if (workshop is null)
                {
                    throw new InvalidOperationException(
                        "The workshop does not exist.");
                }

                await _createEnrollmentUseCase.ExecuteAsync(
                    participant.Id,
                    new CreateEnrollmentRequest
                    {
                        WorkshopId = workshop.Id
                    });

                result.SuccessfulEnrollments++;
            }
            catch (InvalidOperationException exception)
            {
                result.RejectedRows++;

                result.Errors.Add(new EnrollmentImportError
                {
                    RowNumber = row.RowNumber,
                    ParticipantEmail = row.ParticipantEmail,
                    WorkshopCode = row.WorkshopCode,
                    Reason = exception.Message
                });
            }
            catch (ArgumentException exception)
            {
                result.RejectedRows++;

                result.Errors.Add(new EnrollmentImportError
                {
                    RowNumber = row.RowNumber,
                    ParticipantEmail = row.ParticipantEmail,
                    WorkshopCode = row.WorkshopCode,
                    Reason = exception.Message
                });
            }
        }

        return result;
    }
}