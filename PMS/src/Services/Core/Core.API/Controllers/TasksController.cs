using Core.Application.Commands.CompleteTask;
using Core.Application.Commands.CreateTask;
using Core.Application.Commands.UpdateTaskPlan;
using Core.Application.DTOs;
using Core.Application.Queries.GetMyTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wolverine;

namespace Core.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TasksController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var developerId))
            return Unauthorized();

        var command = new CreateTaskCommand(
            developerId,
            request.Title,
            request.PlanHours
        );

        var taskId = await _messageBus.InvokeAsync<Guid>(command);
        return Ok(new { TaskId = taskId, Message = "Task created successfully" });
    }

    [HttpPost("{taskId:guid}/complete")]
    public async Task<IActionResult> CompleteTask(Guid taskId)
    {
        await _messageBus.InvokeAsync(new CompleteTaskCommand(taskId));
        return Ok(new { Message = "Task completed successfully. Developer volatility recalculated." });
    }

    [HttpPut("{taskId:guid}/plan")]
    public async Task<IActionResult> UpdateTaskPlan(Guid taskId, [FromBody] UpdateTaskPlanRequest request)
    {
        await _messageBus.InvokeAsync(new UpdateTaskPlanCommand(taskId, request.NewPlanHours));

        return Ok(new { Message = "Task plan updated successfully." });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTasks([FromServices] IMessageBus messageBus)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var developerId))
        {
            return Unauthorized(new { message = "Не удалось извлечь ID пользователя из токена." });
        }

        var tasks = await messageBus.InvokeAsync<IEnumerable<TaskDto>>(new GetMyTasksQuery(developerId));

        return Ok(tasks);
    }
}
