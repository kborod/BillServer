using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Requests
{
    public class LastMessagesRequestDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public RequestType RequestType => RequestType.LastRequestsList;
        #endregion

        public List<RequestEnvelope> LastRequests { get; set; }

        public LastMessagesRequestDto(List<RequestEnvelope> lastRequests)
        {
            LastRequests = lastRequests;
        }
    }
}
