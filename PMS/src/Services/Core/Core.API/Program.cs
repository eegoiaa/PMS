using Core.API.Configuration;
using Core.API.Hubs;
using Core.Application.Commands.CreateTask;
using Core.Infrastructure.Configuration;
using PMS.Shared.Common.Extensions;
using PMS.Shared.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices();
builder.Host.AddMessaging(builder.Configuration, typeof(CreateTaskCommand).Assembly);

builder.Services.AddRsaJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<TaskHub>("/task-hub");
app.Run();
