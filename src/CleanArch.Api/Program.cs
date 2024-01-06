using System.Reflection;
using CleanArch.Application;
using CleanArch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices(Scope.API_ONLY_SERVICE);
    services.AddInfrastructureServices(Scope.API_ONLY_SERVICE, context);
    services.AddEndpointDefinitions(Assembly.GetExecutingAssembly());
    services.AddSecurityTokenAuthentication();
    services.AddSecurityTokenAuthorization();
});

var app = builder.Build();

app.InitializeDbContext();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.UseAuthentication();
app.UseAuthorization();

app.Run();