using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace TarasK8.SaveSystem
{
    
    public class ObjectConverter : JsonConverter<Object>
    {
        public override bool CanWrite => _canWrite;
        public override bool CanRead => _canRead;

        private bool _canWrite = true;
        private bool _canRead = true;

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (AssetsContainer.CanReference(value, out string guid))
            {
                writer.WriteValue(guid);
            }
            else
            {
                _canWrite = false;
                serializer.Serialize(writer, value);
            }
        }

        public override Object ReadJson(JsonReader reader, System.Type objectType, Object existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            if (AssetsContainer.TryResolveReference(value, out Object asset))
            {
                return asset;
            }
            else
            {
                _canRead = false;
                return serializer.Deserialize<Object>(reader);
            }
        }
    }
    
}