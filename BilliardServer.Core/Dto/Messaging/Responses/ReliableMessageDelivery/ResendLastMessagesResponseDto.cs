using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses
{
    public class ResendLastMessagesResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.ResendLastRequestsResponse;
        #endregion

        public int FromNumberInclusive { get; set; }

        public ResendLastMessagesResponseDto(int fromNumberInclusive)
        {
            FromNumberInclusive = fromNumberInclusive;
        }
    }
}
