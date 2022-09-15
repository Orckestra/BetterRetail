using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Caching;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryGetCategoriesAsync
    {
        private AutoMocker _container;
        private CultureInfo _englishCultureInfo;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _englishCultureInfo = CultureInfo.GetCultureInfo("en-US");

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

            var overtureClientMock = _container.GetMock<IComposerOvertureClient>();
            overtureClientMock
            .Setup(client => client.SendAsync(It.IsNotNull<GetCategoriesV2Request>()))
            .ReturnsAsync(new CategoryList() { Categories = new List<Category>(categoriesToReturn) })
            .Verifiable();

            var cacheProvider = _container.GetMock<ICacheProvider>();
            cacheProvider
                .Setup(provider => provider.GetOrAddAsync(
                    It.IsNotNull<CacheKey>(),
                    It.IsNotNull<Func<Task<CategoryList>>>(),
                    It.IsAny<Func<CategoryList, Task>>(),
                    It.IsAny<CacheKey>()))
                .Returns<CacheKey, Func<Task<CategoryList>>, Func<CategoryList, Task>, CacheKey>(
                    (key, func, arg3, arg4) => func())
                .Verifiable();
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var overtureClientMock = OvertureClientFactory.CreateForGetCategoriesRequest();
            _container.Use(overtureClientMock);
            var repository = _container.CreateInstance<CategoryRepository>();

            // Act
            var result = await repository.GetCategoriesAsync(new GetCategoriesParam { Scope = GetRandom.String(10) });

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(3);

            overtureClientMock.Verify();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Scope_Is_Null_Or_Empty_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            var repository = _container.CreateInstance<CategoryRepository>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => repository.GetCategoriesAsync(new GetCategoriesParam { Scope = scope }));
        }
    }
}
