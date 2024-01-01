namespace CleanArch.Tests;

public class UnitTest
{
    protected readonly IConfiguration Configuration;
    protected readonly IServiceProvider ServicesProvider;

    public UnitTest()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\", "appsettings.Test.json");
        Configuration = new ConfigurationBuilder()
            .AddJsonFile(path)
            .Build();

    ServicesProvider = new ServiceCollection()
        .AddServices()
        .BuildServiceProvider();
    }
}