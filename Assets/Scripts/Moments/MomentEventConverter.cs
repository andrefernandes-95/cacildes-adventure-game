using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AF
{
    public class MomentEventConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(MomentEvent);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            string type = jsonObject["type"]?.ToString();

            MomentEvent result = type switch
            {
                "dialogue" => new DialogueEvent(),
                "wait" => new WaitEvent(),
                _ => throw new NotSupportedException($"Unsupported event type: {type}")
            };

            serializer.Populate(jsonObject.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jsonObj = JObject.FromObject(value, serializer);
            jsonObj.WriteTo(writer);
        }
    }

}
