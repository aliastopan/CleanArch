using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IRegistrationService
{
    Task<Result<UserAccount>> TrySignUpAsync(string username, string email, string password);
}
