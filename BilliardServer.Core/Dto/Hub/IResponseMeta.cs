namespace BilliardServer.Core.Dto.Hub
{
    public interface IResponseMeta
    {
        bool IsRequired { get; }
        ResponseType ResponseType { get; }
    }
}
