namespace BilliardServer.Core.Models
{
    public class User
    {
        public string Id { get; }
        public string Name { get; private set; } = string.Empty;
        public int Avatar { get; private set; } = 1;

        public int Exp { get; private set; }
        public int Rating { get; private set; }

        public int Chips { get; private set; }
        public int Coins { get; private set; }

        public int PartiesCount { get; private set; }
        public int WinPartiesCount { get; private set; }
        public int TotalChipsPrize { get; private set; }

        public User(string id, string name, int avatar,
            int exp,int rating, int chips, int coins,
            int pariesCount, int winPartiesCount, int totalChipsPrize)
        {
            var validateResult = ValidateParamsForNew(name, avatar);
            if (string.IsNullOrEmpty(validateResult) == false)
                throw new Exception(validateResult);

            Id = id;
            Name = name;
            Avatar = avatar;
            Exp = exp;
            Rating = rating;
            Chips = chips;
            Coins = coins;
            PartiesCount = pariesCount;
            WinPartiesCount = winPartiesCount;
            TotalChipsPrize = totalChipsPrize;
        }

        public static string? ValidateParamsForNew(string name, int avatar)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 3 || name.Length > 10)
                return "Name must be between 3 and 10 characters long";
            if (avatar < 0)
                return "Avatar cant be lower than 0";
            return null;
        }

        public bool SetAvatar(int avatarId)
        {
            if (avatarId > 0 && avatarId < 15 == false)
                return false;

            Avatar = avatarId;
            return true;
        }
    }
}
