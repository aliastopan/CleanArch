using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Application.Common.Interfaces.Services;

public interface IUserRegistrationService
{
    Task<Result<UserAccount>> RegisterUserAsync(string username, string email, string password);
}
