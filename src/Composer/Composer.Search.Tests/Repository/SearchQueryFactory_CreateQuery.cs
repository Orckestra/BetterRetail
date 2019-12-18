using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Tests.Repository
{
    [TestFixture]
    public class SearchQueryFactoryCreateQuery
    {
        private IViewModelMetadataRegistry Registry { get; set; }
        private IProductRequestFactory _sut;

        [SetUp]
        public void SetUp()
        {
            Registry = new ViewModelMetadataRegistry();
            _sut = new AutoMocker().CreateInstance<ProductRequestFactory>(); ;
        }

        [Test]
        public void WHEN_creating_query_SHOULD_return_non_null_instance()
        {
            // Act
            var query = _sut.CreateProductRequest(new SearchCriteria() {Scope = "Quebec"});

            // Assert
            query.Should().NotBeNull();
        }
    }
}
