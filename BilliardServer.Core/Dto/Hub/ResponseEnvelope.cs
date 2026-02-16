using System.Text.Json;
using System.Text.Json.Serialization;

namespace BilliardServer.Core.Dto.Hub
{
    /// <summary>
    /// Сообщение с сервера на клиент
    /// </summary>
    public class ResponseEnvelope
    {
        public required bool IsRequired { get; set; }

        public required int SequenceNumber { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required ResponseType ResponseType { get; set; }

        public required JsonElement Payload { get; set; }

        public static ResponseEnvelope Create<T>(T payload) where T : IResponseMeta
        {
            var payloadJson = JsonSerializer.SerializeToElement(payload);
            return new ResponseEnvelope
            {
                IsRequired = payload.IsRequired,
                SequenceNumber = -1,
                ResponseType = payload.ResponseType,
                Payload = payloadJson
            };
        }
    }
}
