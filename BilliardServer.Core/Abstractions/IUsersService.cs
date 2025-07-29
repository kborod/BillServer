using BilliardServer.Core.Models;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersService
    {
        Task<int> Create(User user);
        Task<int> Delete(int id);
        Task<List<User>> GetAll();
        Task<int> Update(int id, string name, int avatar);
    }
}