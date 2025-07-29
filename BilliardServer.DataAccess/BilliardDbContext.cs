using BilliardServer.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilliardServer.DataAccess
{
    public class BilliardDbContext : DbContext
    {
        public BilliardDbContext(DbContextOptions<BilliardDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
    }
}
