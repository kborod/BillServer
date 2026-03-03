using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;

namespace Billiard.Application
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<Result<User?>> GetUser(string id)
        {
            return await _usersRepository.GetUser(id);
        }

        public async Task<Result<User?>> GetByEmail(string email)
        {
            return await _usersRepository.GetByEmail(email);
        }

        public async Task Update(string id, string name, int avatar)
        {
            await _usersRepository.Update(id, name, avatar);
        }

        public async Task Delete(string id)
        {
            await _usersRepository.Delete(id);
        }
    }
}
