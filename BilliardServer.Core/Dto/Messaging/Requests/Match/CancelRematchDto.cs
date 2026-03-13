using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.Match
{
    public class CancelRematchDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.CancelRematch;
        #endregion

        public string MatchId { get; set; }

        public CancelRematchDto(string matchId)
        {
            MatchId = matchId;
        }
    }
}
