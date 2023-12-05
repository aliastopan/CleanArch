using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices((_, services) =>
{
    services.AddEndpointDefinitions(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();