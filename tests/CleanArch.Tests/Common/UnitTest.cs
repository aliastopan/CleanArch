using CleanArch.Tests.Unit;

namespace CleanArch.Tests;

public class UnitTest
{
    protected readonly IConfiguration Configuration;
    protected readonly IServiceProvider ServicesProvider;

    public UnitTest()
    {

        var appSettings = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\", "appsettings.Test.json");

        Configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .AddUserSecrets<ConfigurationTests>()
            .Build();

        ServicesProvider = new ServiceCollection()
            .AddServices(Configuration)
            .BuildServiceProvider();
    }
}