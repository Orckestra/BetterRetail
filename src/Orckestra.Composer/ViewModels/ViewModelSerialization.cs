using System;
using System.Linq;
using Fasterflect;
using Newtonsoft.Json;
using Orckestra.Composer.TypeExtensions;

namespace Orckestra.Composer.ViewModels
{
    public class ViewModelSerialization : JsonConverter
    {
        private readonly IViewModelMetadataRegistry _metadataRegistry;

        public ViewModelSerialization(IViewModelMetadataRegistry metadataRegistry)
        {
            _metadataRegistry = metadataRegistry;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Inherits<BaseViewModel>();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = (BaseViewModel)(existingValue ?? objectType.CreateInstance());

            var metaData = _metadataRegistry.GetViewModelMetadata(objectType).ToArray();

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                var meta = metaData.FirstOrDefault(m => Equals(m.DisplayName, reader.Value));

                if (!reader.Read()) { continue; }

                if (meta != null)
                {
                    var value = reader.TokenType == JsonToken.Null
                        ? meta.PropertyType.GetDefaultValue()
                        : serializer.Deserialize(reader, meta.PropertyType);

                    meta.SetValue(obj, value);
                }
                else
                {
                    reader.Skip();
                }
            }

            return obj;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.
        /// </param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var viewModel = ((BaseViewModel)value).ToDictionary();

                writer.WriteStartObject();

                foreach (var property in viewModel)
                {
                    writer.WritePropertyName(property.Key, true);
                    serializer.Serialize(writer, property.Value);
                }
                writer.WriteEndObject();
            }
        }
    }
}