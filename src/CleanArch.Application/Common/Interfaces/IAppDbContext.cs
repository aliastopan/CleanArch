using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces;

public interface IAppDbContext : IDisposable
{
    DbSet<User> Users { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync();
}
