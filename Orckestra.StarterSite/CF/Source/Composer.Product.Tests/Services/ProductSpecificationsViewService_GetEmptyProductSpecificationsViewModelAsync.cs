using System;
using System.Globalization;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductSpecificationsViewServiceGetEmptyProductSpecificationsViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            SetupComposerContext();
        }

        private void SetupComposerContext()
        {
            var composerContext = new Mock<IComposerRequestContext>();
            composerContext.Setup(context => context.CultureInfo).Returns(CultureInfo.GetCultureInfo("en-US"));

            _container.Use(composerContext);
        }

        [Test]
        public void WHEN_null_parameter_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                productSpecificationsViewService.GetEmptySpecificationsViewModel(null);
            });
        }

        [Test]
        public void WHEN_product_id_is_null_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
                {
                    ProductId = null
                });
            });
        }

        [Test]
        public void WHEN_product_id_is_empty_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
                {
                    ProductId = string.Empty
                });
            });
        }

        [Test]
        public void WHEN_variant_id_is_null_SHOULD_not_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.DoesNotThrow(() =>
            {
                productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
                {
                    ProductId = "product1",
                    VariantId = null
                });
            });
        }

        [Test]
        public void WHEN_variant_id_is_empty_SHOULD_not_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.DoesNotThrow(() =>
            {
                productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
                {
                    ProductId = "product1",
                    VariantId = string.Empty
                });
            });
        }

        [Test]
        public void WHEN_product_and_variant_ids_not_null_or_empty_SHOULD_view_model_contains_product_and_variant_ids()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act
            var model = productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
            {
                ProductId = "product1",
                VariantId = "variant1"
            });

            // Assert
            model.ProductId.Should().BeEquivalentTo("product1");
            model.VariantId.Should().BeEquivalentTo("variant1");
        }

        [Test]
        public void WHEN_product_and_variant_ids_not_null_or_empty_SHOULD_context_contains_kvp_for_product_and_variant_ids()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act
            var model = productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
            {
                ProductId = "product1",
                VariantId = "variant1"
            });

            // Assert
            model.Context.Keys.Should().Contain("productId");
            model.Context.Keys.Should().Contain("variantId");
        }

        [Test]
        public void WHEN_getting_empty_specifications_view_model_SHOULD_group_collection_be_empty()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act
            var model = productSpecificationsViewService.GetEmptySpecificationsViewModel(new GetProductSpecificationsParam
            {
                ProductId = "product1",
                VariantId = "variant1"
            });

            // Assert
            model.Groups.Should().BeEmpty();
        }
    }
}
