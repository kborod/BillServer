using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Requests
{
    public class ResendLastMessagesRequestDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public RequestType RequestType => RequestType.ResendLastResponses;
        #endregion

        public int FromNumberInclusive { get; set; }

        public ResendLastMessagesRequestDto(int fromNumberInclusive)
        {
            FromNumberInclusive = fromNumberInclusive;
        }
    }
}
