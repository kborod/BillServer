using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Models;
using BilliardServer.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilliardServer.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BilliardDbContext _context;

        public UsersRepository(BilliardDbContext context)
        {
            _context = context;
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
