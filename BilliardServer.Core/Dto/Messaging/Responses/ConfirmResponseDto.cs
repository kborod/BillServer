using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses
{
    public class ConfirmResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.ConfirmResponse;
        #endregion

        public string Message { get; set; }

        public ConfirmResponseDto(string message)
        {
            Message = message;
        }
    }
}
