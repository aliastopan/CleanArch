using Microsoft.Extensions.Configuration;

namespace CleanArch.Tests;

public class UnitTest
{
    protected readonly IConfiguration Configuration;

    public UnitTest()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\", "appsettings.Test.json");
        Configuration = new ConfigurationBuilder()
            .AddJsonFile(path)
            .Build();
    }
}