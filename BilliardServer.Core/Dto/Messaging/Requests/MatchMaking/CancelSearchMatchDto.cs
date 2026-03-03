using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.MatchMaking
{
    public class CancelSearchMatchDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.CancelSearchMatch;
        #endregion
    }
}
