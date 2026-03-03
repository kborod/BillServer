using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests
{
    public class TestRequestsDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.Test;
        #endregion

        public string Data { get; set; }

        public TestRequestsDto(string data)
        {
            Data = data;
        }
    }
}
