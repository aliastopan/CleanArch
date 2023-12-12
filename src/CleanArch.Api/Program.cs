using System.Reflection;
using CleanArch.Api;
using CleanArch.Application;
using CleanArch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(context.Configuration, context.HostingEnvironment);
    services.AddEndpointDefinitions(Assembly.GetExecutingAssembly());
    services.AddSecurityTokenAuthentication();
    services.AddSecurityTokenAuthorization();
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.UseAuthentication();
app.UseAuthorization();

app.Run();