using SkillHub.Application.DTOs.Certificates;

namespace SkillHub.Application.Interfaces.Documents;

public interface ICertificateGenerator
{
    Task<byte[]> GenerateAsync(EnrollmentCertificateData certificateData);
}