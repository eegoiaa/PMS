using Core.Application.Commands.CreateTask;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Core.API.Controllers;

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
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var taskId = await _messageBus.InvokeAsync<Guid>(command);
        return Ok(new { TaskId = taskId, Message = "Задача успешно создана" });
    }
}
