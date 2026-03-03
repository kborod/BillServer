using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses
{
    public class AreYouAliveResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.AreYouAliveResponse;
        #endregion
    }
}
