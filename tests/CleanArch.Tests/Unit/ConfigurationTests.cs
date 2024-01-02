namespace CleanArch.Tests.Unit;

public class ConfigurationTests : UnitTest
{
    [Fact]
    public void Configuration_ShouldUseInMemoryDatabase()
    {
        Configuration.GetValue<bool>("UseInMemoryDatabase")
            .Should()
            .BeTrue();
    }

    [Fact]
    public void SecurityTokenSettings_Issuer_ShouldBeCleanArch()
    {
        Configuration["SecurityToken:Issuer"]
            .Should()
            .Be("CleanArch");
    }

    [Fact]
    public void SecurityTokenSettings_Audience_ShouldBeCleanArch()
    {
        Configuration["SecurityToken:Audience"]
            .Should()
            .Be("CleanArch");
    }

    [Fact]
    public void UserSecrets_ApiKey_ShouldNotBeNull()
    {
        Configuration["UserSecrets:ApiKey"]
            .Should()
            .NotBeNull();
    }
}
