using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.Match
{
    public class MatchInitedDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.MatchInited;
        #endregion

        public string MatchId { get; set; }

        public MatchInitedDto(string matchId)
        {
            MatchId = matchId;
        }
    }
}
