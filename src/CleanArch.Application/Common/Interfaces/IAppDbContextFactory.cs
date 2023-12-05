namespace CleanArch.Application.Common.Interfaces;

public interface IAppDbContextFactory<out T> where T : IAppDbContext
{
    T CreateDbContext();
}

