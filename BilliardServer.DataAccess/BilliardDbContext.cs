using BilliardServer.Core.Enums;
using BilliardServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BilliardServer.Infrastructure
{
    public class BilliardDbContext : IdentityDbContext<UserEntity, UserRole, long>
    {
        public BilliardDbContext(DbContextOptions<BilliardDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, Name = UserRoleType.Admin, NormalizedName = UserRoleType.Admin.ToUpper(), ConcurrencyStamp = "666f2167-ace3-4ead-be1a-99e3aa97441d" },
                new UserRole { Id = 2, Name = UserRoleType.User, NormalizedName = UserRoleType.User.ToUpper(), ConcurrencyStamp = "7890eb73-827b-45c8-8b57-bc376c2644f0" }
            );
        }
    }
}
