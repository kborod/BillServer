using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests
{
    public class MessageReceivedRequestDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public RequestType RequestType => RequestType.ResponseReceived;
        #endregion

        public int LastReceivedResponse { get; set; }

        public MessageReceivedRequestDto(int lastReceivedResponse)
        {
            LastReceivedResponse = lastReceivedResponse;
        }
    }
}
