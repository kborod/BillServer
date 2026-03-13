using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using BilliardServer.DataAccess.Extensions;
using BilliardServer.Infrastructure.Entities;
using Kborod.SharedDto;
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

        public async Task<Result<User?>> GetUser(string id)
        {
            var entity = await _userManager.FindByIdAsync(id);
            if (entity == null)
                return Result<User?>.Fail("User not found");
            return Result<User?>.Ok(entity.CreateUser());
        }

        public async Task<Result<User?>> GetByEmail(string email)
        {
            var entity = await _userManager.FindByEmailAsync(email);

            if (entity == null)
                return Result<User?>.Fail("User not found");

            return Result<User?>.Ok(entity.CreateUser());
        }

        public async Task<Result<UserProfile>> GetUserProfile(string id)
        {
            var entity = await _userManager.FindByIdAsync(id);
            if (entity == null)
                return Result<UserProfile>.Fail("user not found");
            else
                return Result<UserProfile>.Ok(new UserProfile { Id = entity.Id.ToString(), Name = entity.Name, Avatar = entity.Avatar });
        }

        public async Task<Result<List<UserProfile>>> GetUserProfiles(List<string> ids)
        {
            var idsLong = ids.Select(id => long.Parse(id));

            var r = await _context.Users
                .AsNoTracking()
                .Where(entity => idsLong.Contains(entity.Id))
                .Select(entity => new UserProfile { Id = entity.Id.ToString(), Name = entity.Name, Avatar = entity.Avatar })
                .ToListAsync();

            return Result<List<UserProfile>>.Ok(r);
        }

        //public async Task Update(string id, string name, int avatar)
        //{
        //    long.TryParse(id, out var userId);
        //    await _context.Users
        //        .Where(u => u.Id == userId)
        //        .ExecuteUpdateAsync(u => u
        //            .SetProperty(u => u.Name, p => name)
        //            .SetProperty(u => u.Avatar, p => avatar)
        //            );
        //}

        //public async Task Delete(string id)
        //{
        //    long.TryParse(id, out var userId);
        //    await _context.Users
        //        .Where(u => u.Id == userId)
        //        .ExecuteDeleteAsync();
        //}
    }
}
