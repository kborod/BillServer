using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Responses
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
