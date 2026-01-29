using BilliardServer.Core.Models;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<User> Create(string name, int avatar);
        Task<int> Delete(int id);
        Task<List<User>> GetAll();
        Task<int> Update(int id, string name, int avatar);
    }
}