using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces;

public interface ISecurityTokenService
{
    string GenerateAccessToken(User user);
}
