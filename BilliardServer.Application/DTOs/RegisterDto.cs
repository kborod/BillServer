namespace Billiard.Application.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }
}