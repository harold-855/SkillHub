using OfficeOpenXml;
using SkillHub.Application.Interfaces.Imports;

namespace SkillHub.Infrastructure.Imports.Excel;

/// <summary>
/// EPPlus implementation of <see cref="IEnrollmentExcelReader"/> that parses rows from the 'Enrollments' sheet.
/// </summary>
public class EpplusEnrollmentExcelReader : IEnrollmentExcelReader
{
    /// <summary>
    /// Initializes the reader and sets non-commercial EPPlus license context.
    /// </summary>
    public EpplusEnrollmentExcelReader()
    {
        // Configure this only if your use qualifies for
        // EPPlus's personal non-commercial license.
        ExcelPackage.License.SetNonCommercialPersonal(
            "Harold Aristizabal");
    }

    /// <summary>
    /// Reads rows from the 'Enrollments' sheet, dynamically locating ParticipantEmail and WorkshopCode headers.
    /// </summary>
    /// <param name="fileStream">The binary stream containing the workbook.</param>
    /// <returns>A collection of parsed import rows.</returns>
    public Task<IReadOnlyCollection<EnrollmentImportRow>> ReadAsync(
        Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        using var package = new ExcelPackage(fileStream);

        var worksheet = package.Workbook.Worksheets["Enrollments"];

        if (worksheet is null)
        {
            throw new InvalidOperationException(
                "The Excel file does not contain an 'Enrollments' worksheet.");
        }

        if (worksheet.Dimension is null)
        {
            return Task.FromResult<IReadOnlyCollection<EnrollmentImportRow>>(
                Array.Empty<EnrollmentImportRow>());
        }

        var participantEmailColumn = 0;
        var workshopCodeColumn = 0;

        // Identify the columns using their header names.
        for (var column = 1;
             column <= worksheet.Dimension.Columns;
             column++)
        {
            var header = worksheet.Cells[1, column]
                .Text.Trim();

            if (header.Equals(
                    "ParticipantEmail",
                    StringComparison.OrdinalIgnoreCase))
            {
                participantEmailColumn = column;
            }

            if (header.Equals(
                    "WorkshopCode",
                    StringComparison.OrdinalIgnoreCase))
            {
                workshopCodeColumn = column;
            }
        }

        if (participantEmailColumn == 0 || workshopCodeColumn == 0)
        {
            throw new InvalidOperationException(
                "The 'Enrollments' worksheet must contain " +
                "'ParticipantEmail' and 'WorkshopCode' columns.");
        }

        var rows = new List<EnrollmentImportRow>();

        // Row 1 contains headers; data begins at row 2.
        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var participantEmail = worksheet
                .Cells[row, participantEmailColumn]
                .Text.Trim();

            var workshopCode = worksheet
                .Cells[row, workshopCodeColumn]
                .Text.Trim();

            // Ignore completely empty rows.
            if (string.IsNullOrWhiteSpace(participantEmail) &&
                string.IsNullOrWhiteSpace(workshopCode))
            {
                continue;
            }

            rows.Add(new EnrollmentImportRow
            {
                RowNumber = row,
                ParticipantEmail = participantEmail,
                WorkshopCode = workshopCode
            });
        }

        return Task.FromResult<IReadOnlyCollection<EnrollmentImportRow>>(
            rows);
    }
}
