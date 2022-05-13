using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Tests.Factory
{
    [TestFixture]
    public class GroceryProductInformationFactory_BuildProductFormat
    {
        private AutoMocker _container;
        private IGroceryProductInformationFactory Factory;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var localizationProviderMock = new Mock<ILocalizationProvider>();

            localizationProviderMock
                .Setup(c => c.GetLocalizedString(It.Is<GetLocalizedParam>(p=> p.Key == "L_Approx")))
                .Returns("approx.").Verifiable();

            localizationProviderMock
              .Setup(c => c.GetLocalizedString(It.Is<GetLocalizedParam>(p => p.Key == "L_EachAbbrev")))
              .Returns("ea.").Verifiable();

            _container.Use(localizationProviderMock);

            Factory = _container.CreateInstance<GroceryProductInformationFactory>();
        }

        [Test]
        [TestCase(0, 0, null, false, null)]
        [TestCase(0, 10, null, false, null)]
        [TestCase(1, 500, "g", false, "500g")]
        [TestCase(2, 500, "g", false, "2 x 500g")]
        [TestCase(1, 500, "ml", false, "500ml")]
        [TestCase(2, 500, "ml", false, "2 x 500ml")]
        [TestCase(1, 180, "g", true, "approx. 180g ea.")]
        [TestCase(2, 180, "g", true, "2 x approx. 180g")]
        public void WHEN_Values_SHOULD_Be_Correct_Format(int productUnitQuantity, decimal productUnitSize, string productUnitMeasure, bool isWeightedProduct, string expectedValue)
        {
            // Act
            var value = Factory.BuildProductFormat(productUnitQuantity, productUnitSize, productUnitMeasure, isWeightedProduct, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(expectedValue, value);
        }
    }
}
