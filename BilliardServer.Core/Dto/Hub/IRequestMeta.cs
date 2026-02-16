namespace BilliardServer.Core.Dto.Hub
{
    public interface IRequestMeta
    {
        /// <summary>
        /// Обязательно должен быть доставлен
        /// </summary>
        bool IsRequired { get; }
        /// <summary>
        /// Тип сообщения
        /// </summary>
        RequestType RequestType { get; }
    }
}
