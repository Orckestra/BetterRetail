using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Product.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    public class RelatedProductViewService_RetrieveRelatedProductsAsync : BaseTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            var repo = Container.GetMock<IProductRepository>();
            repo.Setup(r => r.GetProductAsync(It.IsAny<GetProductParam>()))
                .ReturnsAsync(RelatedProductsViewServiceTestHelper.Product);
        }

        [Test]
        public async Task WHEN_Product_Is_Variant_SHOULD_Link_Variants_To_Products()
        {
            var relatedProductViewService = Container.CreateInstance<RelatedProductViewServiceProxy>();

            var result =
                await
                    relatedProductViewService.RetrieverRelatedProductsAsyncProxy(new GetRelatedProductsParam
                    {
                        ProductIds =
                            new List<ProductIdentifier>
                            {
                                new ProductIdentifier {ProductId = "1", VariantId = "2"}
                            }
                    });

            result.Length.Should().Be(1);
            result.First().Variant.Id.Should().Be("2");
        }
    }
}
