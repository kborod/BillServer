namespace BilliardServer.Core.Models
{
    public class User
    {
        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int Avatar { get; } = 1;

        public User(int id, string name, int avatar)
        {
            var validateResult = ValidateParamsForNew(name, avatar);
            if (string.IsNullOrEmpty(validateResult) == false)
                throw new Exception(validateResult);

            Id = id;
            Name = name;
            Avatar = avatar;
        }

        public static string ValidateParamsForNew(string name, int avatar)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 1)
                return "Name length must be more or equal 1";
            if (avatar < 1)
                return "Avatar cant be lower than 1";
            return null;
        }
    }
}
