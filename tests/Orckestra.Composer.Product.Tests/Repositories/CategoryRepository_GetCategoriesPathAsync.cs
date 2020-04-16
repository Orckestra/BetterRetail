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
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryGetCategoriesPathAsync
    {
        private CultureInfo _cultureInfo;
        private AutoMocker _container;
        private CategoryRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _cultureInfo = CultureInfo.GetCultureInfo("en-US");
            _repository = _container.CreateInstance<CategoryRepository>();

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

            var overtureClientMock = _container.GetMock<IOvertureClient>();
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
            // Act
            var result = await _repository.GetCategoriesPathAsync(new GetCategoriesPathParam { Scope = GetRandom.String(10), CultureInfo = _cultureInfo, CategoryId = "B" });

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result[0].Id.Should().Be("B");
            result[1].Id.Should().Be("A");

            _container.Verify<ICacheProvider>(oc => oc.GetOrAddAsync(
                It.IsNotNull<CacheKey>(),
                It.IsNotNull<Func<Task<CategoryList>>>(),
                It.IsAny<Func<CategoryList, Task>>(),
                It.IsAny<CacheKey>()));
        }

        [TestCase(null, "A")]
        [TestCase("", "A")]
        [TestCase(" ", "A")]
        [TestCase("Quebec", null)]
        [TestCase("Quebec", "")]
        [TestCase("Quebec", " ")]
        public void WHEN_Parameters_Are_Null_Or_Whitespace_SHOULD_Throw_ArgumentException(string scope, string categoryId)
        {
            // Arrange
            _repository = _container.CreateInstance<CategoryRepository>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _repository.GetCategoriesPathAsync(new GetCategoriesPathParam { Scope = scope, CultureInfo = _cultureInfo, CategoryId = categoryId }));
        }

        [Test]
        public void WHEN_CategoryId_Is_Invalid_SHOULD_Throw_ArgumentException()
        {
            _repository = _container.CreateInstance<CategoryRepository>();
            var param = new GetCategoriesPathParam
            {
                Scope = GetRandom.String(10),
                CultureInfo = _cultureInfo,
                CategoryId = "Z"
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _repository.GetCategoriesPathAsync(param));

            // Assert
            exception.ParamName.Should().Be("categoryId");
        }
    }
}
