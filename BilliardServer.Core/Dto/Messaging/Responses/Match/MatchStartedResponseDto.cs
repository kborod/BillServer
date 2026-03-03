using Kborod.BilliardCore;
using Kborod.BilliardCore.Enums;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.Match
{
    public class MatchStartedResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.MatchStartedResponse;
        #endregion

        public StartMatchData StartMatchData { get; set; }

        public MatchStartedResponseDto(StartMatchData startMatchData)
        {
            StartMatchData = startMatchData;
        }
    }
}
