namespace BilliardServer.Core.Dto.Web.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }
}