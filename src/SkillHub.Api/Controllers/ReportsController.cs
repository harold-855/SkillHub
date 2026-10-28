using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillHub.Application.UseCases.Reports.GetTopWorkshops;

namespace SkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Roles = "Employee")]
public class ReportsController : ControllerBase
{
    private readonly GetTopWorkshopsUseCase _getTopWorkshopsUseCase;

    public ReportsController(
        GetTopWorkshopsUseCase getTopWorkshopsUseCase)
    {
        _getTopWorkshopsUseCase = getTopWorkshopsUseCase;
    }

    [HttpGet("top-workshops")]
    public async Task<IActionResult> GetTopWorkshops()
    {
        var report = await _getTopWorkshopsUseCase.ExecuteAsync();

        return Ok(report);
    }
}
