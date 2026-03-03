using Kborod.BilliardCore.Enums;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.MatchMaking
{
    public class SearchMatchDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.SearchMatch;
        #endregion

        public GameType GameType { get; set; }
        public BetType BetType { get; set; }

        public SearchMatchDto(GameType gameType, BetType betType)
        {
            GameType = gameType;
            BetType = betType;
        }
    }
}
