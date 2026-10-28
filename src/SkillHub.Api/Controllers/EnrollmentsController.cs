using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.DTOs.Imports;
using SkillHub.Application.UseCases.Enrollments.CancelEnrollment;
using SkillHub.Application.UseCases.Enrollments.CreateEnrollment;
using SkillHub.Application.UseCases.Enrollments.GenerateEnrollmentCertificate;
using SkillHub.Application.UseCases.Enrollments.GetMyEnrollments;
using SkillHub.Application.UseCases.Enrollments.ImportEnrollments;
using SkillHub.Application.UseCases.Workshops.GetWorkshopEnrollments;

namespace SkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly CreateEnrollmentUseCase _createEnrollmentUseCase;
    private readonly GetMyEnrollmentsUseCase _getMyEnrollmentsUseCase;
    private readonly CancelEnrollmentUseCase _cancelEnrollmentUseCase;
    private readonly ImportEnrollmentsUseCase _importEnrollmentsUseCase;
    private readonly GenerateEnrollmentCertificateUseCase _generateEnrollmentCertificateUseCase;
    private readonly GetWorkshopEnrollmentsUseCase _getWorkshopEnrollmentsUseCase;

    public EnrollmentsController(
        CreateEnrollmentUseCase createEnrollmentUseCase,
        GetMyEnrollmentsUseCase getMyEnrollmentsUseCase,
        CancelEnrollmentUseCase cancelEnrollmentUseCase,
        ImportEnrollmentsUseCase importEnrollmentsUseCase,
        GenerateEnrollmentCertificateUseCase generateEnrollmentCertificateUseCase,
        GetWorkshopEnrollmentsUseCase getWorkshopEnrollmentsUseCase)
    {
        _createEnrollmentUseCase = createEnrollmentUseCase;
        _getMyEnrollmentsUseCase = getMyEnrollmentsUseCase;
        _cancelEnrollmentUseCase = cancelEnrollmentUseCase;
        _importEnrollmentsUseCase = importEnrollmentsUseCase;
        _generateEnrollmentCertificateUseCase = generateEnrollmentCertificateUseCase;
        _getWorkshopEnrollmentsUseCase = getWorkshopEnrollmentsUseCase;
    }

    // POST: api/v1/enrollments
    [HttpPost]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request)
    {
        if (!TryGetCurrentUserId(out var participantId))
        {
            return Unauthorized(new
            {
                message = "The authenticated user's ID is invalid."
            });
        }

        var enrollmentId = await _createEnrollmentUseCase.ExecuteAsync(
            participantId,
            request);

        return StatusCode(StatusCodes.Status201Created, new
        {
            id = enrollmentId,
            message = "Enrollment created successfully."
        });
    }

    // GET: api/v1/enrollments/mine
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyEnrollments()
    {
        if (!TryGetCurrentUserId(out var participantId))
        {
            return Unauthorized(new
            {
                message = "The authenticated user's ID is invalid."
            });
        }

        var enrollments = await _getMyEnrollmentsUseCase.ExecuteAsync(
            participantId);

        return Ok(enrollments);
    }

    // PATCH: api/v1/enrollments/{id}/cancel
    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "Participant,Employee")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelEnrollment(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized(new
            {
                message = "The authenticated user's ID is invalid."
            });
        }

        var isEmployee = User.IsInRole("Employee");

        await _cancelEnrollmentUseCase.ExecuteAsync(
            id,
            currentUserId,
            isEmployee);

        return NoContent();
    }

    // POST: api/v1/enrollments/import
    [HttpPost("import")]
    [Authorize(Roles = "Employee")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ImportEnrollments(
        [FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please select a valid Excel file."
            });
        }

        var extension = Path.GetExtension(file.FileName);

        if (!string.Equals(
                extension,
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Only .xlsx files are supported."
            });
        }

        await using var stream = file.OpenReadStream();

        var result = await _importEnrollmentsUseCase.ExecuteAsync(stream);

        return Ok(result);
    }

    // GET: api/v1/enrollments/{id}/certificate
    [HttpGet("{id:guid}/certificate")]
    [Authorize(Roles = "Participant,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DownloadCertificate(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized(new
            {
                message = "The authenticated user's ID is invalid."
            });
        }

        var isEmployee = User.IsInRole("Employee");

        var pdfBytes =
            await _generateEnrollmentCertificateUseCase.ExecuteAsync(
                id,
                currentUserId,
                isEmployee);

        return File(
            pdfBytes,
            "application/pdf",
            $"SkillHub-Certificate-{id}.pdf");
    }

    // GET: api/v1/workshops/{workshopId}/enrollments
    [HttpGet("/api/v1/workshops/{workshopId:guid}/enrollments")]
    [Authorize(Roles = "Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkshopEnrollments(Guid workshopId)
    {
        var enrollments =
            await _getWorkshopEnrollmentsUseCase.ExecuteAsync(workshopId);

        return Ok(enrollments);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(userIdClaim, out userId);
    }
}