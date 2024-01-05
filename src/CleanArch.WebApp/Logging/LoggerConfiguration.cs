using Serilog;

namespace CleanArch.WebApp.Logging;

public static class LoggerConfiguration
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new Serilog.LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
}
