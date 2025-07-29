namespace BilliardServer.Core.Models
{
    public class User
    {
        public int Id { get; }
        public string Name { get; } = string.Empty;
        public int Avatar { get; } = 1;

        private User(int id, string name, int avatar)
        {
            Id = id;
            Name = name;
            Avatar = avatar;
        }

        public static (User User, string Error) Create(int id, string name, int avatar)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(name))
                error = "Name cant be empty";
            if (avatar < 1)
                error = "Avatar cant be lower than 1";

            var user = new User(id, name, avatar);

            return (user, error);
        }
    }
}
