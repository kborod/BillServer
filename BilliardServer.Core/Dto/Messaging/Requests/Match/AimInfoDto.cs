using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.MatchMaking
{
    public class AimInfoDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public RequestType RequestType => RequestType.AimInfo;
        #endregion

        public AimInfoData AimInfoData { get; set; }

        public AimInfoDto(AimInfoData aimInfoData)
        {
            AimInfoData = aimInfoData;
        }
    }
}
