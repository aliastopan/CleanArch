using System.IdentityModel.Tokens.Jwt;
using CleanArch.Tests.Common.Factories;

namespace CleanArch.Tests.Unit;

public class SecurityTokenTests : UnitTest
{
    private readonly ISecurityTokenService _securityTokenService;
    private string _accessToken;

    public SecurityTokenTests()
    {
        _securityTokenService = base.ServicesProvider.GetRequiredService<ISecurityTokenService>();

        var userAccount = IdentityFactory.GetTestUserAccount();
        _accessToken = _securityTokenService.GenerateAccessToken(userAccount);
    }

    [Fact]
    public void AccessToken_ClaimSub_ShouldBe()
    {
        // arrange
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(_accessToken) as JwtSecurityToken;

        // act
        var sub = jsonToken!.Claims.SingleOrDefault(c => c.Type == "sub")!.Value;

        // assert
        sub.Should().Be("5d771905-c325-4f9a-adb8-954e0ae21860");
    }
}
