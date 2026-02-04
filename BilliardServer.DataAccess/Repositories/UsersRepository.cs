using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using BilliardServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BilliardServer.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BilliardDbContext _context;
        private readonly UserManager<UserEntity> _userManager;

        public UsersRepository(BilliardDbContext context, UserManager<UserEntity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<User?>> GetByEmail(string email)
        {
            var entity = await _userManager.FindByEmailAsync(email);

            if (entity == null)
                return Result<User?>.Fail("User not found");

            return Result<User?>.Ok(new User(entity.Id, entity.Name, entity.Avatar));
        }

        public async Task<List<User>> GetAll()
        {
            var userEntities = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            var users = userEntities
                .Select(e => new User(e.Id, e.Name, e.Avatar))
                .ToList();

            return users;
        }

        public async Task<User> Create(string name, int avatar)
        {
            var userEntity = new UserEntity
            {
                Name = name, 
                Avatar = avatar
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return new User(userEntity.Id, userEntity.Name, userEntity.Avatar);
        }

        public async Task<int> Update(int id, string name, int avatar)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(u => u.Name, p => name)
                    .SetProperty(u => u.Avatar, p => avatar)
                    );

            return id;
        }

        public async Task<int> Delete(int id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
