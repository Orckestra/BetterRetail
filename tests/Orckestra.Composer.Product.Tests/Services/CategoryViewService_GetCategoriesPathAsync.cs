using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class CategoryViewServiceGetCategoriesPathAsync
    {
        private AutoMocker _container;
        private CultureInfo _englishCultureInfo;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _englishCultureInfo = CultureInfo.GetCultureInfo("en-US");
        }

        [Test]
        public void WHEN_Param_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var categoryService = _container.CreateInstance<CategoryViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.GetCategoriesPathAsync(null));
        }

        [Test]
        public void WHEN_Culture_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var categoryService = _container.CreateInstance<CategoryViewService>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.GetCategoriesPathAsync(new GetCategoriesPathParam { Scope = "Quebec", CultureInfo = null, CategoryId = "A" }));
        }

        [Test]
        public async Task WHEN_Parameters_Are_Valid_SHOULD_Return_Categories()
        {
            // Arrange
            var repoMock = CreateCategoryRepositoryMock();
            _container.Use(repoMock);

            var mapperMock = CreateViewModelMapperMock();
            _container.Use(mapperMock);

            var categoryService = _container.CreateInstance<CategoryViewService>();
            // Act
            var result = await categoryService.GetCategoriesPathAsync(new GetCategoriesPathParam { Scope = GetRandom.String(10), CultureInfo = _englishCultureInfo, CategoryId = "B" });

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Id.Should().Be("B");
            result[1].Id.Should().Be("A");
        }

        private Mock<ICategoryRepository> CreateCategoryRepositoryMock()
        {
            var categoriesToReturn = new[]
            {
                new Category
                {
                    Id = "B",
                    PrimaryParentCategoryId = "A",
                    PropertyBag = new PropertyBag()
                },
                new Category
                {
                    Id = "A",
                    PrimaryParentCategoryId = null,
                    PropertyBag = new PropertyBag()
                }
            };

            var repoMock = new Mock<ICategoryRepository>();

            repoMock.Setup(repo => repo.GetCategoriesPathAsync(It.IsAny<GetCategoriesPathParam>()))
                .ReturnsAsync(new List<Category>(categoriesToReturn))
                .Verifiable();

            return repoMock;
        }


        private Mock<IViewModelMapper> CreateViewModelMapperMock()
        {
            var mapperMock = new Mock<IViewModelMapper>();

            mapperMock.Setup(repo => repo.MapTo<CategoryViewModel>(It.Is<Category>(c => c.Id == "A"), It.IsAny<CultureInfo>(), "CAD"))
                .Returns(new CategoryViewModel { Id = "A" })
                .Verifiable();

            mapperMock.Setup(repo => repo.MapTo<CategoryViewModel>(It.Is<Category>(c => c.Id == "B"), It.IsAny<CultureInfo>(), "CAD"))
                .Returns(new CategoryViewModel { Id = "B" })
                .Verifiable();

            return mapperMock;
        }
    }
}
