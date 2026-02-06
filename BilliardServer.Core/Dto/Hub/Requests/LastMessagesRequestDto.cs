using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Requests
{
    public class LastMessagesRequestDto : IRequestMeta
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
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
