using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.MatchMaking
{
    public class ShotResultDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.ShotResult;
        #endregion

        public SyncronizationInfo SynchronizationInfo { get; set; }

        public ShotResultDto(SyncronizationInfo synchronizationInfo)
        {
            SynchronizationInfo = synchronizationInfo;
        }
    }
}
