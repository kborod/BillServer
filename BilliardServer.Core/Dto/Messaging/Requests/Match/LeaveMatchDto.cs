using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.Match
{
    public class LeaveMatchDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.LeaveMatch;
        #endregion

        public string MatchId { get; set; }

        public LeaveMatchDto(string matchId)
        {
            MatchId = matchId;
        }
    }
}
