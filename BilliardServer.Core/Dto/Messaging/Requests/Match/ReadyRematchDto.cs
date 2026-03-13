using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.Match
{
    public class ReadyRematchDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.ReadyRematch;
        #endregion

        public string MatchId { get; set; }

        public ReadyRematchDto(string matchId)
        {
            MatchId = matchId;
        }
    }
}
