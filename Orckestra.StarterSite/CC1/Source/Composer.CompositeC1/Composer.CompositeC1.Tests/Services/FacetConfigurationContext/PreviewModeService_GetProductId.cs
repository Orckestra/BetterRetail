using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Search;

// ReSharper disable InconsistentNaming

namespace Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext
{
    [TestFixture]
    public class PreviewModeService_GetProductId : FacetTestsBase
    {
        private PreviewModeService _target;

        // mocks
        private Mock<HttpContextBase> _httpContextMoq;
        private Mock<IDataQueryService> _dataQueryMoq;
        private Mock<ICacheStore<Guid, string>> _cacheMoq;
        private Mock<IProductRepository> _productRepoMoq;
        private Mock<ISearchRepository> _searchRepoMoq;
        private Mock<IComposerContext> _composerContextMoq;

        // storage
        private List<IPreviewModeMeta> _previewModes;

        private const string PageKey = "PageRenderer.IPage";
        private Guid _pageId;

        [SetUp]
        public void SetUp()
        {
            // IPage
            _pageId = Guid.Empty;
            var pageMoq = new Mock<IPage>();
            pageMoq.Setup(x => x.Id).Returns(() => _pageId);

            // HttpContextBase
            _httpContextMoq = new Mock<HttpContextBase> { CallBase = true };
            _httpContextMoq
                .Setup(x => x.Items)
                .Returns(() => new Dictionary<string, object>
                {
                    [PageKey] = pageMoq.Object,
                });

            // IProductRepository
            _productRepoMoq = new Mock<IProductRepository>();
            _productRepoMoq
                .Setup(q => q.GetProductAsync(It.IsAny<GetProductParam>()))
                .Returns(Task.FromResult(null as Overture.ServiceModel.Products.Product));

            // ISearchRepository
            _searchRepoMoq = new Mock<ISearchRepository>();

            // IComposerContext
            _composerContextMoq = new Mock<IComposerContext>();
            _composerContextMoq.Setup(q => q.CultureInfo).Returns(CultureInfo.InvariantCulture);
            _composerContextMoq.Setup(q => q.Scope).Returns("BetterRetailerCanada");

            // cache
            var cacheServiceMoq = new Mock<ICacheService>();
            _cacheMoq = new Mock<ICacheStore<Guid, string>>();
            _cacheMoq
                .Setup(q => q.GetOrAdd(It.IsAny<Guid>(), It.IsAny<Func<Guid, string>>()))
                .Returns<Guid, Func<Guid, string>>((key, factory) => factory(key));
            cacheServiceMoq
                .Setup(q => q.GetStoreWithDependencies<Guid, string>(It.IsAny<string>(), It.IsAny<CacheDependentEntry[]>()))
                .Returns(_cacheMoq.Object);

            // storage
            _dataQueryMoq = new Mock<IDataQueryService>();
            _previewModes = new List<IPreviewModeMeta>();
            _dataQueryMoq.Setup(q => q.Get<IPreviewModeMeta>()).Returns(() => _previewModes.AsQueryable());

            // test target
            _target = new PreviewModeService(_httpContextMoq.Object, cacheServiceMoq.Object, _dataQueryMoq.Object,
                _productRepoMoq.Object, _searchRepoMoq.Object, _composerContextMoq.Object);
        }

        [Test]
        public void WHEN_productId_cached_SHOULD_return_cached_value()
        {
            // arrange
            var cachedProductId = Guid.NewGuid().ToString();
            _cacheMoq
                .Setup(q => q.GetOrAdd(It.IsAny<Guid>(), It.IsAny<Func<Guid, string>>()))
                .Returns(cachedProductId);

            // act
            var productId = _target.GetProductId();

            // assert
            productId.Should().Be(cachedProductId);
        }

        [Test]
        public void WHEN_valid_product_is_set_on_page_SHOULD_return_this_product()
        {
            // arrange
            var configuredProductId = Guid.NewGuid().ToString();
            _pageId = Guid.NewGuid();
            _previewModes.Add(new PreviewModeMetaImpl
            {
                PageId = _pageId,
                ProductId = configuredProductId,
            });
            SetupProductRepository(configuredProductId);

            // act
            var productId = _target.GetProductId();

            // assert
            productId.Should().Be(configuredProductId);
        }

        [Test]
        public void WHEN_invalid_product_is_set_on_page_SHOULD_return_any_product()
        {
            // arrange
            var configuredProductId = Guid.NewGuid().ToString();
            _pageId = Guid.NewGuid();
            _previewModes.Add(new PreviewModeMetaImpl
            {
                PageId = _pageId,
                ProductId = configuredProductId,
            });

            var searchedProductId = Guid.NewGuid().ToString();
            SetupSearchedProduct(searchedProductId);

            // act
            var productId = _target.GetProductId();

            // assert
            productId.Should().Be(searchedProductId);
        }

        [Test]
        public void WHEN_product_is_not_set_on_page_SHOULD_return_any_product()
        {
            // arrange
            _pageId = Guid.NewGuid();
            _previewModes.Add(new PreviewModeMetaImpl
            {
                PageId = _pageId,
                ProductId = null,
            });

            var searchedProductId = Guid.NewGuid().ToString();
            SetupSearchedProduct(searchedProductId);

            // act
            var productId = _target.GetProductId();

            // assert
            productId.Should().Be(searchedProductId);
        }

        [Test]
        public void WHEN_pageId_cant_be_obtained_SHOULD_return_any_product()
        {
            // arrange
            _httpContextMoq
                .Setup(x => x.Items)
                .Returns(() => new Dictionary<string, object>());

            var searchedProductId = Guid.NewGuid().ToString();
            SetupSearchedProduct(searchedProductId);

            // act
            var productId = _target.GetProductId();

            // assert
            productId.Should().Be(searchedProductId);
        }


        private void SetupSearchedProduct(string searchedProductId)
        {
            _searchRepoMoq
                .Setup(q => q.SearchProductAsync(It.IsAny<SearchCriteria>()))
                .Returns(Task.FromResult(new ProductSearchResult
                {
                    Documents = new List<ProductDocument>
                    {
                        new ProductDocument
                        {
                            ProductId = searchedProductId
                        }
                    }
                }));
        }

        private void SetupProductRepository(string productId)
        {
            _productRepoMoq
                .Setup(q => q.GetProductAsync(It.Is<GetProductParam>(p => p.ProductId == productId)))
                .Returns(Task.FromResult(new Overture.ServiceModel.Products.Product { Id = productId }));
        }

        private class PreviewModeMetaImpl : IPreviewModeMeta
        {
            public DataSourceId DataSourceId { get; set; }
            public Guid Id { get; set; }
            public Guid PageId { get; set; }
            public string PublicationStatus { get; set; }
            public Guid VersionId { get; set; }
            public string FieldName { get; set; }
            public string ProductId { get; set; }
        }
    };
}