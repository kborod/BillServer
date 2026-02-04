using BilliardServer.Core.Models;
using BilliardServer.Infrastructure.Entities;

namespace BilliardServer.DataAccess.Extensions
{
    internal static class UserExtensions
    {
        public static User CreateUser(this UserEntity entity)
        {
            return new User(entity.Id, entity.Name, entity.Avatar);
        }

    }
}
