using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Services;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Search;
using Orckestra.Overture.ServiceModel.Search.Pricing;

namespace Orckestra.Composer.Search.Tests.Providers
{
    [TestFixture]
    public class FromPriceProvider_GetPriceAsync
    {
        private AutoMocker _container;
        private readonly CultureInfo _cultureInfo = TestingExtensions.GetRandomCulture();
        private const string Scope = "global";

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var composerContext = new Mock<IComposerContext>();
            composerContext.Setup(context => context.CultureInfo).Returns(_cultureInfo);
            composerContext.Setup(context => context.Scope).Returns(Scope);
            composerContext.SetupGet(mock => mock.CurrencyIso).Returns("CAD");
            _container.Use(composerContext);

            var localizationProvider = new Mock<ILocalizationProvider>();
            localizationProvider.Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>())).Returns("{0:C}").Verifiable();
            _container.Use(localizationProvider);
        }

        [Test]
        public async Task WHEN_product_has_NO_variant_and_is_in_discount_SHOULD_return_price_and_discount_price()
        {
            // Arrange
            const bool hasVariants = false;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag(),
                EntityPricing = new EntityPricing()
                {
                    RegularPrice = 15,
                    CurrentPrice = 7.99
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeFalse();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(15);
            priceSearchViewModel.Price.Should().Be(7.99);
            priceSearchViewModel.DisplayPrice.Should().NotBeNullOrEmpty();
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_NO_variant_and_is_NOT_in_discount_SHOULD_return_regular_price()
        {
            // Arrange
            const bool hasVariants = false;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag(),
                EntityPricing = new EntityPricing()
                {
                    RegularPrice = 15,
                    CurrentPrice = 15
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeFalse();
            priceSearchViewModel.IsOnSale.Should().BeFalse();
            priceSearchViewModel.ListPrice.Should().Be(15);
            priceSearchViewModel.Price.Should().Be(null);
            priceSearchViewModel.DisplayPrice.Should().NotBeNullOrEmpty();
            priceSearchViewModel.DisplaySpecialPrice.Should().Be(null);
        }

        [Test]
        public async Task WHEN_product_has_variant_and_has_NO_variants_in_discount_AND_all_variants_regular_prices_are_the_same_SHOULD_return_regular_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 49.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeFalse();
            priceSearchViewModel.IsOnSale.Should().BeFalse();
            priceSearchViewModel.ListPrice.Should().Be(49.99);
            priceSearchViewModel.Price.Should().Be(null);
            priceSearchViewModel.DisplayPrice.Should().NotBeNullOrEmpty();
            priceSearchViewModel.DisplaySpecialPrice.Should().Be(null);
        }

        [Test]
        public async Task WHEN_product_has_variant_and_has_NO_variants_in_discount_AND_all_variants_regular_prices_are_NOT_the_same_SHOULD_return_FROM_minimum_regular_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 54.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeFalse();
            priceSearchViewModel.ListPrice.Should().Be(49.99);
            priceSearchViewModel.Price.Should().Be(null);
            priceSearchViewModel.DisplayPrice.Should().NotBeNullOrEmpty();
            priceSearchViewModel.DisplaySpecialPrice.Should().Be(null);
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_NOT_the_same_AND_all_variant_discount_prices_are_the_same_SHOULD_return_FROM_minimum_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 44.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 44.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(null);
            priceSearchViewModel.Price.Should().Be(44.99);
            priceSearchViewModel.DisplayPrice.Should().Be(null);
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_NOT_the_same_AND_all_variant_discount_prices_are_the_same_SHOULD_return_FROM_minimum_price2()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 44.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 44.99},
                    //Improbable but we cover this case
                    {FromPriceProvider.GroupRegularPriceFromProperty, 39.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(null);
            priceSearchViewModel.Price.Should().Be(39.99);
            priceSearchViewModel.DisplayPrice.Should().Be(null);
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_NOT_the_same_AND_all_variant_discount_prices_are_NOT_the_same_SHOULD_return_FROM_minimum_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 44.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(null);
            priceSearchViewModel.Price.Should().Be(44.99);
            priceSearchViewModel.DisplayPrice.Should().Be(null);
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_NOT_the_same_AND_all_variant_discount_prices_are_NOT_the_same_SHOULD_return_FROM_minimum_price2()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 44.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 49.99},
                    //Improbable but we cover this case
                    {FromPriceProvider.GroupRegularPriceFromProperty, 39.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(null);
            priceSearchViewModel.Price.Should().Be(39.99);
            priceSearchViewModel.DisplayPrice.Should().Be(null);
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_the_same_AND_all_variant_discount_prices_are_the_same_SHOULD_return_regular_and_discount_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 49.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 54.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeFalse();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(54.99);
            priceSearchViewModel.Price.Should().Be(49.99);
            priceSearchViewModel.DisplayPrice.Should().NotBeNullOrEmpty();
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task WHEN_product_has_variants_and_at_least_one_in_discount_AND_all_variants_regular_prices_are_the_same_AND_all_variant_discount_prices_are_NOT_the_same_SHOULD_return_from_minimum_discount_price()
        {
            // Arrange
            const bool hasVariants = true;
            var provider = _container.CreateInstance<FromPriceProvider>();

            var document = new ProductDocument
            {
                PropertyBag = new PropertyBag
                {
                    {FromPriceProvider.GroupCurrentPriceFromProperty, 39.99},
                    {FromPriceProvider.GroupCurrentPriceToProperty, 49.99},
                    {FromPriceProvider.GroupRegularPriceFromProperty, 54.99},
                    {FromPriceProvider.GroupRegularPriceToProperty, 54.99}
                }
            };

            // Act
            var priceSearchViewModel = await provider.GetPriceAsync(hasVariants, document);

            // Assert
            priceSearchViewModel.HasPriceRange.Should().BeTrue();
            priceSearchViewModel.IsOnSale.Should().BeTrue();
            priceSearchViewModel.ListPrice.Should().Be(null);
            priceSearchViewModel.Price.Should().Be(39.99);
            priceSearchViewModel.DisplayPrice.Should().Be(null);
            priceSearchViewModel.DisplaySpecialPrice.Should().NotBeNullOrEmpty();
        }
    }
}