using System.IO;
using Newtonsoft.Json;

namespace Securities
{
    public class JsonDeserializer
    {
        private readonly JsonSerializer _serializer;

        public JsonDeserializer() => _serializer = JsonSerializer.Create();

        public T Deserialize<T>(string json)
        {
            T ret;

            using (var reader     = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                ret = _serializer.Deserialize<T>(jsonReader);
            }

            return ret;
        }
    }
}
