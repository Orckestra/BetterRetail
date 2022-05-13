using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelSerialization
{
    public class SetupForViewModelSerializationTest : BaseTestForStaticSut
    {
        protected TestViewModelForSerialization ViewModelForSerialization;
        protected Composer.ViewModels.ViewModelMapper ViewModelMapper;
        protected Mock<IViewModelMetadataRegistry> MetadataRegistry;
        protected string JsonFromCustomSerializer;
        private List<IPropertyMetadata> _metadata;
        protected string JsonFromStandardSerializer;
        protected TestViewModelForSerialization DeserializedTestViewModelForSerialization { get; set; }

        [SetUp]
        protected void Setup()
        {
            MetadataRegistry = Container.GetMock<IViewModelMetadataRegistry>();
            ViewModelMetadataRegistry.Current = MetadataRegistry.Object;
            ViewModelForSerialization = TestViewModelForSerializationFactory.Create();

            var sourceType = ViewModelForSerialization.GetType();
            _metadata = new List<IPropertyMetadata>();

            foreach (var propertyInfo in sourceType.GetProperties())
            {
                _metadata.Add(new InstancePropertyMetadata(propertyInfo));
            }
            Dependency<IViewModelMetadataRegistry>().Setup(m => m.GetViewModelMetadata(typeof(TestViewModelForSerialization))).Returns(_metadata);

            var lookupService = Container.GetMock<ILookupService>();
            var localizationProviderMock = Container.GetMock<ILocalizationProvider>();
            var currencyProviderMock = Container.GetMock<ICurrencyProvider>();
            ViewModelMapper = new Composer.ViewModels.ViewModelMapper(MetadataRegistry.Object, Dependency<IViewModelPropertyFormatter>().Object, lookupService.Object, localizationProviderMock.Object, currencyProviderMock.Object);

            JsonFromStandardSerializer = JsonConvert.SerializeObject(ViewModelForSerialization, Formatting.Indented);
        }
    }
    public class ViewModelSerializationReadJson : SetupForViewModelSerializationTest
    {
        [Test]
        public void WHEN_deserializing_using_custom_composer_json_converter_all_properties_SHOULD_be_the_same_as_in_source_object()
        {
            DeserializedTestViewModelForSerialization = JsonConvert.DeserializeObject<TestViewModelForSerialization>(JsonFromStandardSerializer, new Composer.ViewModels.ViewModelSerialization(MetadataRegistry.Object));

            ViewModelForSerialization.ShouldBeEquivalentTo(DeserializedTestViewModelForSerialization);
            ViewModelForSerialization.TestNested.ShouldBeEquivalentTo(DeserializedTestViewModelForSerialization.TestNested);
            ViewModelForSerialization.TestBag.ShouldAllBeEquivalentTo(DeserializedTestViewModelForSerialization.TestBag);
            ViewModelForSerialization.TestNested.TestBag.ShouldAllBeEquivalentTo(DeserializedTestViewModelForSerialization.TestNested.TestBag);
        }
    }
}