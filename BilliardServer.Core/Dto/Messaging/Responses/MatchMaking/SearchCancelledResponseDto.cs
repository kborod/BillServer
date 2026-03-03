using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.MatchMaking
{
    public class SearchCancelledResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.SearchCancelledResponse;
        #endregion
    }
}
