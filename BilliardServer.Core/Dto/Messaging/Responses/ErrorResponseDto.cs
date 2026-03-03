using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses
{
    public class ErrorResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.ErrorResponse;
        #endregion

        public string Error { get; set; }

        public ErrorResponseDto(string error)
        {
            Error = error;
        }
    }
}
