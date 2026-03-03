using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests
{
    public class StartSessionDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public RequestType RequestType => RequestType.StartSession;
        #endregion
    }
}
