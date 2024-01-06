using Microsoft.FluentUI.AspNetCore.Components;
using CleanArch.Application;
using CleanArch.Infrastructure;
using CleanArch.WebApp.Components;
using CleanArch.WebApp.Logging;
using CleanArch.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices(ServiceScope.WEBAPP_ONLY_SERVICE);
    services.AddInfrastructureServices(ServiceScope.WEBAPP_ONLY_SERVICE, context);
    services.AddRazorComponents()
            .AddInteractiveServerComponents();
    services.AddFluentUIComponents();
    services.AddHttpContextAccessor();
    services.AddHttpClient<IdentityClientService>((_, httpClient) =>
    {
        httpClient.BaseAddress = new Uri("https://localhost:7244");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
