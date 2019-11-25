using Newtonsoft.Json;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Basic Json Serializer for Composer
    /// </summary>
    public class ComposerJsonSerializer : IComposerJsonSerializer
    {
        /// <summary>
        /// Deserialize a json in the type of the generic T.
        /// </summary>
        /// <param name="json">The json to deserialize</param>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, ComposerHost.Current == null ? new JsonSerializerSettings() : ComposerHost.Current.JsonSettings);
        }

        /// <summary>
        /// Serialize a generic T in a Json string.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, ComposerHost.Current == null ? new JsonSerializerSettings() : ComposerHost.Current.JsonSettings);
        }
    }
}
