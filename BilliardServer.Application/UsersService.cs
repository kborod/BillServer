using BilliardServer.Core.Abstractions;
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

        public async Task<List<User>> GetAll()
        {
            return await _usersRepository.GetAll();
        }

        public async Task<int> Create(User user)
        {
            return await _usersRepository.Create(user);
        }

        public async Task<int> Update(int id, string name, int avatar)
        {
            return await _usersRepository.Update(id, name, avatar);
        }

        public async Task<int> Delete(int id)
        {
            return await _usersRepository.Delete(id);
        }
    }
}
