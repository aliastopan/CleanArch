using System.Security.Claims;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IAccessTokenService
{
    string GenerateAccessToken(UserAccount user);
    Result TryValidateAccessToken(string accessToken);
    ClaimsPrincipal? GetPrincipalFromToken(string accessToken);
}
