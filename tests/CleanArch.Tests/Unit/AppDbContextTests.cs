namespace CleanArch.Tests.Unit;

public class AppDbContextTests : UnitTest
{
    private readonly IDataSeedingService ctxSeeder;

    public AppDbContextTests()
    {
        ctxSeeder = base.ServicesProvider.GetRequiredService<IDataSeedingService>();
    }

    [Fact]
    public async Task AppDbContext_SeederTest()
    {
        (await ctxSeeder.GenerateUsersAsync())
            .Should()
            .BeGreaterThan(0);
    }
}
