using Kborod.BilliardCore;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Messaging.Responses.Match
{
    public class StartTurnResponseDto : IResponse
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.StartTurnResponse;
        #endregion

        public StartTurnData StartTurnData { get; set; }

        public StartTurnResponseDto(StartTurnData startTurnData)
        {
            StartTurnData = startTurnData;
        }
    }
}
