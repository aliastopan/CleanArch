namespace CleanArch.Tests.Unit;

public class AppDbContextTests : UnitTest
{
    private readonly IAppDbContextSeeder ctxSeeder;

    public AppDbContextTests()
    {
        ctxSeeder = base.ServicesProvider.GetRequiredService<IAppDbContextSeeder>();
    }

    [Fact]
    public async Task AppDbContext_SeederTest()
    {
        (await ctxSeeder.GenerateUsersAsync())
            .Should()
            .BeGreaterThan(0);
    }
}
