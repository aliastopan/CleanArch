using Microsoft.FluentUI.AspNetCore.Components;
using CleanArch.Application;
using CleanArch.Infrastructure;
using CleanArch.WebApp.Components;
using CleanArch.WebApp.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(context.Configuration, context.HostingEnvironment);
    services.AddRazorComponents()
            .AddInteractiveServerComponents();
    services.AddFluentUIComponents();
    services.AddHttpContextAccessor();
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
