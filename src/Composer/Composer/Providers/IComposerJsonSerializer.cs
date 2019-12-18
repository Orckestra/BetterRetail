namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Basic Json Serializer for Composer
    /// </summary>
    public interface IComposerJsonSerializer
    {
        /// <summary>
        /// Deserialize a json in the type of the generic T.
        /// </summary>
        /// <param name="json">The json to deserialize</param>
        T Deserialize<T>(string json);

        /// <summary>
        /// Serialize a generic T in a Json string.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        string Serialize<T>(T value);
    }
}
