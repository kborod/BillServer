using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using Kborod.SharedDto;

namespace Billiard.Application
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public Task<Result<User?>> GetUser(string id)
        {
            return _usersRepository.GetUser(id);
        }

        public Task<Result<User?>> GetByEmail(string email)
        {
            return _usersRepository.GetByEmail(email);
        }

        public Task<Result<UserProfile>> GetUserProfile(string id)
        {
            return _usersRepository.GetUserProfile(id);
        }

        public Task<Result<List<UserProfile>>> GetUserProfiles(List<string> ids)
        {
            return _usersRepository.GetUserProfiles(ids);
        }

        public async Task<Result> SetAvatar(string userId, int avatarId)
        {
            var result = await _usersRepository.GetUser(userId);

            if (result.IsSuccess == false)
                return Result.Fail(result.Error!);

            var user = result.Value!;
            if (user.SetAvatar(avatarId) == false)
                return Result.Fail("User cant use this avatar");

            await _usersRepository.UpdateAvatar(user.Id, avatarId);

            return Result.Ok();
        }
    }
}
