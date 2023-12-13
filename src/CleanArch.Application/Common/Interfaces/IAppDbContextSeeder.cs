namespace CleanArch.Application.Common.Interfaces;

public interface IAppDbContextSeeder
{
    Task<int> GenerateUsersAsync();
}
