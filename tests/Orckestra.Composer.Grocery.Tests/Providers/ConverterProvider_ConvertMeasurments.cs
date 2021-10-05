using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Grocery.Providers;

namespace Orckestra.Composer.Grocery.Tests.Providers
{
    [TestFixture]
    public class ConverterProvider_ConvertMeasurments
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TearDown]
        public void TearDown()
        {
            _container.VerifyAll();
        }

        [Test]
        [TestCase(1, "Kilogram", "Gram", 1000)]
        [TestCase(1, "Litre", "Kilogram", 1)]
        [TestCase(1, "NotExistentToMeasure", "Kilogram", 1)]
        [TestCase(1, "Kilogram", "NotExistentFromMeasure", 1)]
        [TestCase(1, "NotExistentToMeasure", "NotExistentFromMeasure", 1)]
        public void WHEN_ConvertMeasurements_SHOULD_Be_CorrectValue(int size,string fromMeasure, string toMeasure ,int expectedValue)
        {
            //Arrange
            IConverterProvider converterProvider =
                _container.CreateInstance<ConverterProvider>();

            // Act
            var value = converterProvider.ConvertMeasurements(size, fromMeasure, toMeasure);

            // Assert
            value.Should().NotBe(null);
            value.Should().Equals(expectedValue);
        }
    }
}
