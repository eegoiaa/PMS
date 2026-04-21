using Core.Application.Queries.GetAnalytics;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IMessageBus _messageBus; 
    public AnalyticsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpGet("developer/{developerId}")]
    public async Task<IActionResult> GetDeveloperLoad(Guid developerId)
    {
        var result = await _messageBus.InvokeAsync<GetDeveloperAnalyticsDto>(new GetDeveloperAnalyticsQuery(developerId));
        return Ok(result);
    }
}
