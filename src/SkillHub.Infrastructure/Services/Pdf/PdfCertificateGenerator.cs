using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkillHub.Application.DTOs.Certificates;
using SkillHub.Application.Interfaces.Documents;

namespace SkillHub.Infrastructure.Services.Pdf;

public class PdfCertificateGenerator : ICertificateGenerator
{
    public Task<byte[]> GenerateAsync(
        EnrollmentCertificateData certificateData)
    {
        ArgumentNullException.ThrowIfNull(certificateData);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(45);
                page.DefaultTextStyle(style => style.FontSize(11));

                page.Header()
                    .PaddingBottom(15)
                    .Column(column =>
                    {
                        column.Item()
                            .Text("SkillHub")
                            .FontSize(28)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        column.Item()
                            .Text("WORKSHOP LEARNING PLATFORM")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(18);

                        column.Item()
                            .BorderBottom(2)
                            .BorderColor(Colors.Blue.Darken2)
                            .PaddingBottom(12)
                            .Text("Constancia de inscripción")
                            .FontSize(22)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        column.Item()
                            .Text(
                                "Por medio del presente documento, se certifica " +
                                "que la siguiente persona se encuentra inscrita " +
                                "en el taller indicado.")
                            .FontSize(12);

                        column.Item()
                            .Background(Colors.Grey.Lighten4)
                            .Padding(15)
                            .Column(details =>
                            {
                                details.Spacing(10);

                                details.Item()
                                    .Text("DATOS DEL PARTICIPANTE")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                details.Item()
                                    .Text($"Nombre: {certificateData.ParticipantName}");

                                details.Item()
                                    .Text($"Correo electrónico: {certificateData.ParticipantEmail}");
                            });

                        column.Item()
                            .Column(details =>
                            {
                                details.Spacing(10);

                                details.Item()
                                    .Text("INFORMACIÓN DEL TALLER")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                details.Item()
                                    .Text($"Código: {certificateData.WorkshopCode}");

                                details.Item()
                                    .Text($"Título: {certificateData.WorkshopTitle}");

                                details.Item()
                                    .Text($"Instructor: {certificateData.InstructorName}");

                                details.Item()
                                    .Text(
                                        $"Fecha y hora de inicio: " +
                                        $"{certificateData.WorkshopStartDateTime:dd/MM/yyyy HH:mm}");

                                details.Item()
                                    .Text(
                                        $"Fecha y hora de finalización: " +
                                        $"{certificateData.WorkshopEndDateTime:dd/MM/yyyy HH:mm}");
                            });

                        column.Item()
                            .PaddingTop(10)
                            .Column(details =>
                            {
                                details.Spacing(8);

                                details.Item()
                                    .Text(
                                        $"Fecha de inscripción: " +
                                        $"{certificateData.EnrolledAt:dd/MM/yyyy HH:mm}");

                                details.Item()
                                    .Text(
                                        $"Fecha de generación: " +
                                        $"{certificateData.GeneratedAt:dd/MM/yyyy HH:mm}");
                            });

                        column.Item()
                            .PaddingTop(15)
                            .Text(
                                "Esta constancia acredita la inscripción al taller. " +
                                "No constituye una certificación de asistencia o aprobación.")
                            .FontSize(9)
                            .Italic()
                            .FontColor(Colors.Grey.Darken1);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("SkillHub | Constancia de inscripción")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });
            });
        });

        byte[] pdfBytes = document.GeneratePdf();

        return Task.FromResult(pdfBytes);
    }
}