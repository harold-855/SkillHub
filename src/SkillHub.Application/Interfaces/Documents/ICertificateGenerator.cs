using SkillHub.Application.DTOs.Certificates;

namespace SkillHub.Application.Interfaces.Documents;

/// <summary>
/// Service abstraction for generating official enrollment proof certificates in PDF format.
/// </summary>
public interface ICertificateGenerator
{
    /// <summary>
    /// Renders a PDF certificate document byte array from enrollment certificate data.
    /// </summary>
    Task<byte[]> GenerateAsync(EnrollmentCertificateData certificateData);
}
