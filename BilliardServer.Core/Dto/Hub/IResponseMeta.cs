namespace BilliardServer.Core.Dto.Hub
{
    public interface IResponseMeta
    {
        /// <summary>
        /// Обязательно должен быть доставлен
        /// </summary>
        bool IsRequired { get; }
        /// <summary>
        /// Тип сообщения
        /// </summary>
        ResponseType ResponseType { get; }
    }
}
