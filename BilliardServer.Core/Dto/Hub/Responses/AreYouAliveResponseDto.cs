using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Responses
{
    public class AreYouAliveResponseDto : IResponseMeta
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => false;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.TestResponse;
        #endregion

        public string Data { get; set; }

        public AreYouAliveResponseDto(string data)
        {
            Data = data;
        }
    }
}
