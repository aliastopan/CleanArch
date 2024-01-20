namespace CleanArch.Application.Common.Interfaces.Services;

public interface IDataSeedingService
{
    Task<int> GenerateUsersAsync();
}
