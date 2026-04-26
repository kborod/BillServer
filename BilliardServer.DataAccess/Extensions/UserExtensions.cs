using BilliardServer.Core.Models;
using BilliardServer.Infrastructure.Entities;

namespace BilliardServer.DataAccess.Extensions
{
    internal static class UserExtensions
    {
        public static User CreateUser(this UserEntity entity)
        {
            return new User(entity.Id.ToString(), entity.Name, entity.Avatar, entity.Exp, entity.Rating, 
                entity.Chips, entity.Coins, entity.PartiesCount, entity.WinPartiesCount, entity.TotalChipsPrize);
        }

    }
}