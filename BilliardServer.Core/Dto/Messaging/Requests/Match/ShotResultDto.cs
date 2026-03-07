using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.Match
{
    public class ShotResultDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.ShotResult;
        #endregion

        public SynchronizationInfo SynchronizationInfo { get; set; }

        public ShotResultDto(SynchronizationInfo synchronizationInfo)
        {
            SynchronizationInfo = synchronizationInfo;
        }
    }
}
