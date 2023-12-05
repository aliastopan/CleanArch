using Microsoft.EntityFrameworkCore;
using CleanArch.Domain.Entities.Identity;

namespace CleanArch.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }

    int SaveChanges();
}
