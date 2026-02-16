using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Responses
{
    public class ErrorResponseDto : IResponseMeta
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
