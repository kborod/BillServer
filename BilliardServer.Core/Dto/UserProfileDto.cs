namespace BilliardServer.Core.Dto
{
    public class UserProfileDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? AvatarUrl { get; set; }

        public UserProfileDto(string id, string username, string? avatarUrl = null)
        {
            Id = id;
            Username = username;
            AvatarUrl = avatarUrl;
        }
    }
}
