using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Responses
{
    public class MessageReceivedResponseDto : IResponseMeta
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.MessageReceivedResponse;
        #endregion

        public int LastReceivedRequest { get; set; }

        public MessageReceivedResponseDto(int lastReceivedRequest)
        {
            LastReceivedRequest = lastReceivedRequest;
        }
    }
}
