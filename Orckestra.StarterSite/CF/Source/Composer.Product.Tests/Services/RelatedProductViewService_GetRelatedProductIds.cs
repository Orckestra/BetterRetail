using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.ForTests;

namespace Orckestra.Composer.Product.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    public class RelatedProductViewService_GetRelatedProductIds : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            var productRepository = Container.GetMock<IProductRepository>();
            productRepository.Setup(m => m.GetProductAsync(It.IsAny<GetProductParam>())).ReturnsAsync(RelatedProductsViewServiceTestHelper.Product);
        }
        [Test]
        public async Task WHEN_MerchandiseType_IS_CrossSell_SHOULD_Return_CrossSell_Related_Products()
        {
            var relatedProductService = Container.CreateInstance<RelatedProductViewService>();

            var result = await relatedProductService.GetProductIdsAsync(new GetProductIdentifiersParam
            {
                ProductId = "1",
                CultureInfo = new CultureInfo("en-CA"),
                MerchandiseTypes = new[] { MerchandiseType.CrossSell },
                Scope = "Canada",
                MaxItems = 4
            });

            result.Should().NotBeNull();
            result.Context["ProductIdentifiers"].Should().NotBeNull();
            var identifiers = result.Context["ProductIdentifiers"] as IEnumerable<ProductIdentifier>;
            identifiers.Should().NotBeNull();
            identifiers.Count().Should().Be(4);
        }

        [Test]
        public async Task WHEN_Relationship_Is_Variant_Product_Id_SHOULD_Be_Set_From_VariantProductId()
        {
            var relatedProductService = Container.CreateInstance<RelatedProductViewService>();

            var result = await relatedProductService.GetProductIdsAsync(new GetProductIdentifiersParam
            {
                ProductId = "1",
                CultureInfo = new CultureInfo("en-CA"),
                MerchandiseTypes = new[] { MerchandiseType.CrossSell },
                Scope = "Canada",
                MaxItems = 4
            });

            var identifiers = result.Context["ProductIdentifiers"] as IEnumerable<ProductIdentifier>;
            // normal product relationship
            identifiers.Should().Contain(p => p.ProductId == "111");
            // variant product
            identifiers.Should().Contain(p => p.ProductId == "555");
            identifiers.Single(p => p.ProductId == "555").VariantId.Should().Be("444");

        }

        [Test]
        public async Task WHEN_Relationships_Have_Sort_Order_Products_SHOULD_Be_Sorted()
        {
            var relatedProductService = Container.CreateInstance<RelatedProductViewService>();

            var result = await relatedProductService.GetProductIdsAsync(new GetProductIdentifiersParam
            {
                ProductId = "1",
                CultureInfo = new CultureInfo("en-CA"),
                MerchandiseTypes = new[] { MerchandiseType.CrossSell },
                Scope = "Canada",
                MaxItems = 4
            });

            var identifiers = result.Context["ProductIdentifiers"] as IEnumerable<ProductIdentifier>;
            identifiers.First().ProductId.Should().Be("555");
            identifiers.Skip(3).Take(1).First().ProductId.Should().Be("888");
        }

        [Test]
        public async Task WHEN_There_Are_More_Than_Four_Relationships_Four_SHOULD_Be_Returned()
        {
            var relatedProductService = Container.CreateInstance<RelatedProductViewService>();

            var result = await relatedProductService.GetProductIdsAsync(new GetProductIdentifiersParam
            {
                ProductId = "1",
                CultureInfo = new CultureInfo("en-CA"),
                MerchandiseTypes = new[] { MerchandiseType.CrossSell },
                Scope = "Canada",
                MaxItems = 4
            });

            var identifiers = result.Context["ProductIdentifiers"] as IEnumerable<ProductIdentifier>;
            identifiers.Count().Should().Be(4);
        }

        [Test]
        public async Task WHEN_requested_product_does_not_exist_SHOULD_return_empty_list()
        {
            var unknownProductId = GetRandom.String(32);

            Container.GetMock<IProductRepository>()
                .Setup(m => m.GetProductAsync(It.Is<GetProductParam>(param => param.ProductId == unknownProductId)))
                .ReturnsAsync(null);

            var relatedProductService = Container.CreateInstance<RelatedProductViewService>();

            var result = await relatedProductService.GetProductIdsAsync(new GetProductIdentifiersParam
            {
                ProductId = unknownProductId,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                MerchandiseTypes = new[] { MerchandiseType.CrossSell },
                Scope = GetRandom.String(32),
                MaxItems = 4
            });

            var identifiers = result.Context["ProductIdentifiers"] as IEnumerable<ProductIdentifier>;
            identifiers.Should().BeEmpty("Because an unknown product cannot have related products");
        }
    }
}
