namespace BilliardServer.API.DTOs
{
    public record UserRequest
    (
        int Id,
        string Name,
        int Avatar
    );
}
