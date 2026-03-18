using Gamism.SDK.Core.Network.Json;
using Newtonsoft.Json;

namespace Gamism.SDK.Unity.Network.Json
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings;
        }

        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _settings);

        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, _settings);
    }
}
