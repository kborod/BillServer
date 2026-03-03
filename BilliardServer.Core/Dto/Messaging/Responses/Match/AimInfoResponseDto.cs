using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.Match
{
    public class AimInfoResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.AimInfoResponse;
        #endregion

        public AimInfoData AimInfoData { get; set; }

        public AimInfoResponseDto(AimInfoData aimInfoData)
        {
            AimInfoData = aimInfoData;
        }
    }
}
