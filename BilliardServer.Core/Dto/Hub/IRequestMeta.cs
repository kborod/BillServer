namespace BilliardServer.Core.Dto.Hub
{
    public interface IRequestMeta
    {
        bool IsRequired { get; }
        RequestType RequestType { get; }
    }
}
