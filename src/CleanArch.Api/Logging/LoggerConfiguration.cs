using Microsoft.IdentityModel.Tokens;
using Serilog.Events;

namespace CleanArch.Api.Logging;

public static class LoggerConfiguration
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new Serilog.LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .Filter.ByExcluding(logEvent =>
                logEvent.Exception is SecurityTokenExpiredException &&
                logEvent.Level == LogEventLevel.Information)
            .Filter.ByExcluding(logEvent =>
                logEvent.MessageTemplate.Text.Contains("Generated query execution expression"))
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
}
