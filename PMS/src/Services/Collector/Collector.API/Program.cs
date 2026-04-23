using Collector.Application.Jobs;
using Collector.Infrastructure.Configuration;
using Hangfire;
using PMS.Shared.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Host.AddMessaging(builder.Configuration, typeof(GitHubCollectorJob).Assembly);

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

app.UseHangfireDashboard();

// 4. Регистрируем фоновую задачу (Job)
// Hangfire сам инжектит все зависимости (GitProvider, DbContext) при вызове метода
RecurringJob.AddOrUpdate<GitHubCollectorJob>(
    "github-sync-job",
    job => job.SyncCommitsAsync(CancellationToken.None), 
    "*/5 * * * *" // CRON выражение: запускать каждые 5 минут
);

app.Run();
