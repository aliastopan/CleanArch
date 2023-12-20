namespace CleanArch.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<Result> TryGrantRoleAsync(Guid userAccountId, string role);
}
