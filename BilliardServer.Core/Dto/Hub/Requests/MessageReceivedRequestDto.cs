using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Requests
{
    public class MessageReceivedRequestDto : IRequestMeta
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
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
