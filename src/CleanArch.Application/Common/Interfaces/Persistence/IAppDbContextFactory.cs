namespace CleanArch.Application.Common.Interfaces.Persistence;

public interface IAppDbContextFactory<out T> where T : IAppDbContext
{
    T CreateDbContext();
}

