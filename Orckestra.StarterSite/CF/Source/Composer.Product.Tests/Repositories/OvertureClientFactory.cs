using System.Collections.Generic;
using Moq;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    public static class OvertureClientFactory
    {
        public static Mock<IOvertureClient> CreateForGetCategoriesRequest()
        {
            var categoriesToReturn = new[]
            {
                new Category
                {
                    Id = "A",
                    PrimaryParentCategoryId = null,
                    PropertyBag = new PropertyBag()
                },
                new Category
                {
                    Id = "B",
                    PrimaryParentCategoryId = "A",
                    PropertyBag = new PropertyBag()
                },
                new Category
                {
                    Id = "C",
                    PrimaryParentCategoryId = "A",
                    PropertyBag = new PropertyBag()
                }
            };

            var overtureClientMock = new Mock<IOvertureClient>();
            overtureClientMock
            .Setup(client => client.SendAsync(It.IsNotNull<GetCategoriesRequest>()))
            .ReturnsAsync(new List<Category>(categoriesToReturn))
            .Verifiable();
            return overtureClientMock;
        }
    }
}
