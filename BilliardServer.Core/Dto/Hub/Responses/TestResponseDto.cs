using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub.Responses
{
    public class TestResponseDto : IResponseMeta
    {
        #region IResponseMeta
        [JsonIgnore]
        public bool IsRequired => true;
        [JsonIgnore]
        public ResponseType ResponseType => ResponseType.TestResponse;
        #endregion

        public string Data { get; set; }

        public TestResponseDto(string data)
        {
            Data = data;
        }
    }
}
