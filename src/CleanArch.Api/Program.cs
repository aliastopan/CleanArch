using System.Reflection;
using CleanArch.Application;
using CleanArch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(context.Configuration);
    services.AddEndpointDefinitions(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();