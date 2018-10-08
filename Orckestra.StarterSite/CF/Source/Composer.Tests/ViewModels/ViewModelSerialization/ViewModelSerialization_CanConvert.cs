using FluentAssertions;
using NUnit.Framework;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelSerialization
{
    public class ViewModelSerializationTestCanConvert : BaseTestForAutocreatedSutOfType<Composer.ViewModels.ViewModelSerialization>
    {
        [Test]
        public void WHEN_trying_to_serialize_object_Can_Convert_SHOULD_be_false()
        {
            var sut = Container.CreateInstance<Composer.ViewModels.ViewModelSerialization>();
            bool canConvert = sut.CanConvert(typeof (object));

            canConvert.Should().BeFalse();
        }
        [Test]
        public void WHEN_trying_to_serialize_class_derived_from_Base_View_Model_Can_Convert_SHOULD_be_true()
        {
            var sut = Container.CreateInstance<Composer.ViewModels.ViewModelSerialization>();
            bool canConvert = sut.CanConvert(typeof(TestViewModelForSerialization));

            canConvert.Should().BeTrue();
        }

    }
}