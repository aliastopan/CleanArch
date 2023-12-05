using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Host.ConfigureServices((_, services) =>
{
    services.AddEndpointDefinitions(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();