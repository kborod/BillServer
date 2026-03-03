using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.Match
{
    public class MakeShotResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.MakeShotResponse;
        #endregion

        public MakeShotData MakeShotData { get; set; }

        public MakeShotResponseDto(MakeShotData makeShotData)
        {
            MakeShotData = makeShotData;
        }
    }
}
