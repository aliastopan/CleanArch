using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IUserRegistrationService
{
    Task<Result<UserAccount>> TryRegisterUserAsync(string username, string email, string password);
}
