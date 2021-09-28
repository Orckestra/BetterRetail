using Newtonsoft.Json;
using Orckestra.Composer.Providers;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    /// <summary>
    /// Basic Json Serializer for Composer
    /// </summary>
    internal class ComposerJsonSerializerMock : IComposerJsonSerializer
    {
        private readonly ViewModelSerialization _vmSerialization;

        public ComposerJsonSerializerMock(IViewModelMetadataRegistry registry, IViewModelMapper modelMapper)
        {
            _vmSerialization = new ViewModelSerialization(registry);
        }

        /// <summary>
        /// Deserialize a json in the type of the generic T.
        /// </summary>
        /// <param name="json">The json to deserialize</param>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _vmSerialization);
        }

        /// <summary>
        /// Serialize a generic T in a Json string.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _vmSerialization);

        }
    }
}