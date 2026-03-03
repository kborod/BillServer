using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses
{
    public class LastMessagesResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.LastResponsesListResponse;
        #endregion

        public List<ResponseEnvelope> LastResponses { get; set; }

        public LastMessagesResponseDto(List<ResponseEnvelope> lastResponses)
        {
            LastResponses = lastResponses;
        }
    }
}
