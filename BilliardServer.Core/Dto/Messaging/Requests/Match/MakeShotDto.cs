using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Requests.MatchMaking
{
    public class MakeShotDto : IRequest
    {
        #region IRequestMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public RequestType RequestType => RequestType.MakeShot;
        #endregion

        public MakeShotData MakeShotData { get; set; }

        public MakeShotDto(MakeShotData makeShotData)
        {
            MakeShotData = makeShotData;
        }
    }
}
