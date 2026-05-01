using PMS.Gateway.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGatewayInfrastructure();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapReverseProxy();

app.Run();