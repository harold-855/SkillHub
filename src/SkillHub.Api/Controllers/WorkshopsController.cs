using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.UseCases.Workshops.CreateWorkshop;
using SkillHub.Application.UseCases.Workshops.GetAvailableWorkshops;
using SkillHub.Application.UseCases.Workshops.GetWorkshopById;

namespace SkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/workshops")]
public class WorkshopsController : ControllerBase
{
    private readonly CreateWorkshopUseCase _createWorkshopUseCase;
    private readonly GetAvailableWorkshopsUseCase _getAvailableWorkshopsUseCase;
    private readonly GetWorkshopByIdUseCase _getWorkshopByIdUseCase;

    public WorkshopsController(
        CreateWorkshopUseCase createWorkshopUseCase,
        GetAvailableWorkshopsUseCase getAvailableWorkshopsUseCase,
        GetWorkshopByIdUseCase getWorkshopByIdUseCase)
    {
        _createWorkshopUseCase = createWorkshopUseCase;
        _getAvailableWorkshopsUseCase = getAvailableWorkshopsUseCase;
        _getWorkshopByIdUseCase = getWorkshopByIdUseCase;
    }

    // GET: api/v1/workshops
    // Public endpoint: anyone can view available workshops.
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableWorkshops()
    {
        var workshops =
            await _getAvailableWorkshopsUseCase.ExecuteAsync();

        return Ok(workshops);
    }

    // GET: api/v1/workshops/{id}
    // Public endpoint: anyone can view a workshop's details.
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkshopById(Guid id)
    {
        var workshop =
            await _getWorkshopByIdUseCase.ExecuteAsync(id);

        if (workshop is null)
        {
            return NotFound(new
            {
                message = "The requested workshop was not found."
            });
        }

        return Ok(workshop);
    }

    // POST: api/v1/workshops
    // Only authenticated employees can create workshops.
    [HttpPost]
    [Authorize(Roles = "Employee")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateWorkshop(
        [FromBody] CreateWorkshopRequest request)
    {
        var workshopId =
            await _createWorkshopUseCase.ExecuteAsync(request);

        return CreatedAtAction(
            nameof(GetWorkshopById),
            new { id = workshopId },
            new { id = workshopId });
    }
}
