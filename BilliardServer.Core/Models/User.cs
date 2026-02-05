namespace BilliardServer.Core.Models
{
    public class User
    {
        public long Id { get; }
        public string Name { get; } = string.Empty;
        public int Avatar { get; } = 1;

        public User(long id, string name, int avatar)
        {
            var validateResult = ValidateParamsForNew(name, avatar);
            if (string.IsNullOrEmpty(validateResult) == false)
                throw new Exception(validateResult);

            Id = id;
            Name = name;
            Avatar = avatar;
        }

        public static string? ValidateParamsForNew(string name, int avatar)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 3 || name.Length > 10)
                return "Name must be between 3 and 10 characters long";
            if (avatar < 0)
                return "Avatar cant be lower than 0";
            return null;
        }
    }
}
