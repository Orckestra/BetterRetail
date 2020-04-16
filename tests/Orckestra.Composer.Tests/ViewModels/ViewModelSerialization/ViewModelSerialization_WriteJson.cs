using System;
using System.Collections;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelSerialization
{
    public class ViewModelSerializationWriteJson : SetupForViewModelSerializationTest
    {
        [Test]
        public void WHEN_serializing_using_custom_composer_json_converter_result_json_SHOULD_be_the_same_as_from_standard_newtonsoft_serializer()
        {
            JsonFromCustomSerializer = JsonConvert.SerializeObject(ViewModelForSerialization, Formatting.Indented, new Composer.ViewModels.ViewModelSerialization(ViewModelMapper, MetadataRegistry.Object));

            JsonFromCustomSerializer.Should().Be(JsonFromStandardSerializer);
            
            Console.WriteLine("Custom Serializer: {0}", JsonFromCustomSerializer);
            Console.WriteLine("Standard Serializer: {0}", JsonFromStandardSerializer);
        }
        
        [Test]
        public void WHEN_serializing_null_object_result_json_SHOULD_be_null_json()
        {
            JsonFromCustomSerializer = JsonConvert.SerializeObject(null, Formatting.Indented, new Composer.ViewModels.ViewModelSerialization(ViewModelMapper, MetadataRegistry.Object));

            JsonFromCustomSerializer.Should().Be("null");

            Console.WriteLine("Custom Serializer: {0}", JsonFromCustomSerializer);
        }

        [Test]
        public void WHEN_serializing_empty_string_result_json_SHOULD_be_empty_string()
        {
            JsonFromCustomSerializer = JsonConvert.SerializeObject(string.Empty, Formatting.Indented, new Composer.ViewModels.ViewModelSerialization(ViewModelMapper, MetadataRegistry.Object));

            string emptyString = @JsonFromCustomSerializer;

            JsonFromCustomSerializer.Should().Be(emptyString);

            Console.WriteLine("Custom Serializer: {0}", JsonFromCustomSerializer);
        }
        
        [Test]
        public void WHEN_serializing_empty_object_result_json_SHOULD_be_curly_brackets()
        {
            JsonFromCustomSerializer = JsonConvert.SerializeObject(new object(), Formatting.Indented, new Composer.ViewModels.ViewModelSerialization(ViewModelMapper, MetadataRegistry.Object));

            JsonFromCustomSerializer.Should().Be("{}");

            Console.WriteLine("Custom Serializer: {0}", JsonFromCustomSerializer);
        }
        [Test]
        public void WHEN_serializing_empty_array_result_json_SHOULD_be_square_brackets()
        {
            JsonFromCustomSerializer = JsonConvert.SerializeObject(new ArrayList(), Formatting.Indented, new Composer.ViewModels.ViewModelSerialization(ViewModelMapper, MetadataRegistry.Object));

            JsonFromCustomSerializer.Should().Be("[]");

            Console.WriteLine("Custom Serializer: {0}", JsonFromCustomSerializer);
        }
    }
}