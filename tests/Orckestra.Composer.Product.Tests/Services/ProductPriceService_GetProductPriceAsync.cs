using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.Tests.Mock;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductPriceServiceGetProductPriceAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateDamProvider());
            _container.Use(FakeViewModelMapper.CreateFake(typeof(ProductPriceViewService).Assembly));
            _container.Use(CreateLocalizationProvider());
        }

        [Test]
        public async Task WHEN_ProductId_Is_Valid_With_Variants_SHOULD_Return_List_Of_Products_Prices_With_Variants()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithProductAndVariantPrices());
            var productService = _container.CreateInstance<ProductPriceViewService>();

            //Act
            var model = await productService.CalculatePricesAsync(new GetProductsPriceParam
            {
                CultureInfo = CultureInfo.CreateSpecificCulture("en-US"),
                Scope = GetRandom.String(32),
                ProductIds = new List<string>
                {
                    GetRandom.String(32),
                    GetRandom.String(32)
                }
            }).ConfigureAwait(false);

            //Assert
            model.Should().NotBeNull();
            model.ProductPrices.Should().NotBeNullOrEmpty();
            model.ProductPrices.Count.Should().Be(2);

            model.ProductPrices[0].IsPriceDiscounted.Should().Be(true);
            model.ProductPrices[0].DefaultListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[0].ListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[0].ProductId = "ABC123";

            model.ProductPrices[0].VariantPrices.Should().NotBeNull();
            model.ProductPrices[0].VariantPrices.Count.Should().Be(1);
            model.ProductPrices[0].VariantPrices[0].IsPriceDiscounted.Should().Be(true);
            model.ProductPrices[0].VariantPrices[0].ListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[0].VariantPrices[0].DefaultListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[0].VariantPrices[0].VariantId.Should().Be("VAR1D");

            model.ProductPrices[1].IsPriceDiscounted.Should().Be(false);
            model.ProductPrices[1].DefaultListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[1].ListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[1].ProductId = "DEF123";

            model.ProductPrices[1].VariantPrices.Should().NotBeNull();
            model.ProductPrices[1].VariantPrices.Count.Should().Be(1);
            model.ProductPrices[1].VariantPrices[0].IsPriceDiscounted.Should().Be(false);
            model.ProductPrices[1].VariantPrices[0].ListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[1].VariantPrices[0].DefaultListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[1].VariantPrices[0].VariantId.Should().Be("VAR2D");
        }

        [Test]
        public async Task WHEN_ProductId_With_NoVariant_Is_Valid_SHOULD_Return_List_Of_One_Products_Prices_With_No_Variant()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithProductPriceNoVariant());
            var productService = _container.CreateInstance<ProductPriceViewService>();

            //Act
            var model = await productService.CalculatePricesAsync(new GetProductsPriceParam
            {
                CultureInfo = CultureInfo.CreateSpecificCulture("en-US"),
                Scope = GetRandom.String(32),
                ProductIds = new List<string>
                {
                    GetRandom.String(32),
                    GetRandom.String(32)
                }
            }).ConfigureAwait(false);

            //Assert
            model.Should().NotBeNull();
            model.ProductPrices.Should().NotBeNullOrEmpty();
            model.ProductPrices.Count.Should().Be(1);

            model.ProductPrices[0].DefaultListPrice.Should().NotBeNullOrWhiteSpace();
            model.ProductPrices[0].ProductId = "ABC123";

            model.ProductPrices[0].VariantPrices.Should().NotBeNull();
            model.ProductPrices[0].VariantPrices.Count.Should().Be(0);
        }

        [Test]
        public void WHEN_ProductPriceParam_Is_Null_SHOULD_Throw_Null_Arguments_Exception()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithProductPriceNoVariant());
            var productService = _container.CreateInstance<ProductPriceViewService>();
            var param = new GetProductsPriceParam();

            //Act
            var action = new Action(async () => await productService.CalculatePricesAsync(param));

            // Act
            Expression<Func<Task<ProductsPricesViewModel>>> expression = () => productService.CalculatePricesAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [Test]
        public void WHEN_Scope_Is_Null_SHOULD_Throw_Arguments_Exception()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithProductPriceNoVariant());
            var productService = _container.CreateInstance<ProductPriceViewService>();
            var param = new GetProductsPriceParam
            {
                CultureInfo = CultureInfo.CreateSpecificCulture("en-US"),
                ProductIds = new List<string>
                    {
                        GetRandom.String(32),
                        GetRandom.String(32)
                    }
            };

            // Act
            Expression<Func<Task<ProductsPricesViewModel>>> expression = () => productService.CalculatePricesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullEmpty(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_Culture_Is_Null_SHOULD_Throw_Arguments_Exception()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithProductPriceNoVariant());
            var productService = _container.CreateInstance<ProductPriceViewService>();
            var param = new GetProductsPriceParam
            {
                Scope = GetRandom.String(32),
                ProductIds = new List<string>
                    {
                        GetRandom.String(32),
                        GetRandom.String(32)
                    }
            };

            // Act
            Expression<Func<Task<ProductsPricesViewModel>>> expression = () => productService.CalculatePricesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        #region Mock
        private static Mock<IDamProvider> CreateDamProvider()
        {
            var damProvider = new Mock<IDamProvider>();

            return damProvider;
        }

        private static Mock<IProductRepository> CreateProductRepositoryWithProductAndVariantPrices()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            
            var productPriceList = new List<ProductPrice>();

            productPriceList.Add(new ProductPrice
            {
                ProductId = "ABC123",
                DefaultPrice = 123.456m,
                Pricing = new ProductPriceEntry {Price = 122.456m},
                VariantPrices = new List<VariantPrice>
                {
                    new VariantPrice
                    {
                        DefaultPrice = 234.567m,
                        VariantId = "VAR1D",
                        Pricing = new ProductPriceEntry
                        {
                            Price = 156.789m
                        }
                    }
                }
            });

            productPriceList.Add(new ProductPrice
            {
                ProductId = "DEF123",
                DefaultPrice = 123.456m,
                Pricing = new ProductPriceEntry { Price = 123.456m },
                VariantPrices = new List<VariantPrice>
                {
                    new VariantPrice
                    {
                        DefaultPrice = 4.567m,
                        VariantId = "VAR2D",
                        Pricing = new ProductPriceEntry
                        {
                            Price = 4.567m
                        }
                    }
                }
            });

            productRepositoryMock.Setup(repo => repo.CalculatePricesAsync(It.IsAny<List<string>>(), It.IsAny<string>()))
                                 .ReturnsAsync(productPriceList)
                                 .Verifiable();

            return productRepositoryMock;
        }

        private static Mock<IProductRepository> CreateProductRepositoryWithProductPriceNoVariant()
        {
            var productRepositoryMock = new Mock<IProductRepository>();

            var productPriceList = new List<ProductPrice>();

            productPriceList.Add(new ProductPrice
            {
                ProductId = "ABC123",
                DefaultPrice = 123.456m,
                Pricing = new ProductPriceEntry { Price = 123.456m },
                VariantPrices = new List<VariantPrice>()
            });

            productRepositoryMock.Setup(repo => repo.CalculatePricesAsync(It.IsAny<List<string>>(), It.IsAny<string>()))
                                 .ReturnsAsync(productPriceList)
                                 .Verifiable();

            return productRepositoryMock;
        }

        private static Mock<ILocalizationProvider> CreateLocalizationProvider()
        {
            var localizationProvider = new Mock<ILocalizationProvider>();
            localizationProvider.Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>())).Returns("{0:C}").Verifiable();

            return localizationProvider;
        }
        #endregion
    }
}
