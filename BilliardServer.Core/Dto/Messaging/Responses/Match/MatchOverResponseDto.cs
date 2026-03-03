using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.Match
{
    public class MatchOverResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.MatchOverResponse;
        #endregion

        public MatchOverData MatchOverData { get; set; }

        public MatchOverResponseDto(MatchOverData matchOverData)
        {
            MatchOverData = matchOverData;
        }
    }
}
