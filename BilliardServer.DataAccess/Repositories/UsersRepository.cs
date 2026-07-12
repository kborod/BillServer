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

        public async Task<Result> UpdateAvatar(string userId, int avatarId)
        {
            long.TryParse(userId, out var id);
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(u => u.Avatar, p => avatarId)
                    );

            return Result.Ok();
        }

        public async Task<Result> UpdateAfterMatch(string userId, int expDelta, int ratingDelta, bool isWin, int chipsPrize, int matchesCountDelta = 1)
        {
            long.TryParse(userId, out var id);
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(u => u.Exp, p => EF.Functions.Greatest(p.Exp + expDelta, 0))
                    .SetProperty(u => u.Rating, p => EF.Functions.Greatest(p.Rating + ratingDelta, 0))
                    .SetProperty(u => u.WinPartiesCount, p => p.WinPartiesCount + (isWin ? 1 : 0))
                    .SetProperty(u => u.TotalChipsPrize, p => p.TotalChipsPrize + chipsPrize)
                    .SetProperty(u => u.PartiesCount, p => p.PartiesCount + matchesCountDelta)
                    );

            return Result.Ok();
        }

        public async Task<List<User>> GetAllUsersForLeaderboard()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(entity => new User(
                    entity.Id.ToString(),
                    entity.Name,
                    entity.Avatar,
                    entity.Exp,
                    entity.Rating,
                    entity.Chips,
                    entity.Coins,
                    entity.PartiesCount,
                    entity.WinPartiesCount,
                    entity.TotalChipsPrize
                ))
                .ToListAsync();

            return users;
        }

        public async void Test()
        {
            var t = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return "name";
            });


            var t2 = Task.Run(async () =>
            {
                await Task.Delay(1000);
                return 5;
            });

            var r = Task.WhenAny(t, t2);
        }
    }
}
