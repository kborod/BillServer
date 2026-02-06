using System.Text.Json;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub
{
    /// <summary>
    /// Сообщение с клиента на сервер
    /// </summary>
    public class RequestEnvelope
    {
        public bool IsRequired { get; set; }

        public int SequenceNumber { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestType RequestType { get; set; }

        public JsonElement Payload { get; set; }

        public static RequestEnvelope Create<T>(T payload) where T : IRequestMeta
        {
            var payloadJson = JsonSerializer.SerializeToElement(payload);
            return new RequestEnvelope
            {
                IsRequired = payload.IsRequired,
                SequenceNumber = 0,
                RequestType = payload.RequestType,
                Payload = payloadJson
            };
        }
    }
}
