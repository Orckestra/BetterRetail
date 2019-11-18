using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Composite.Data.Types;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.CompositeC1.Services.Facet;
using Orckestra.Composer.Search;

// ReSharper disable InconsistentNaming

namespace Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext
{
    [TestFixture]
    public class FacetConfigurationContext_GetFacetSettings : FacetTestsBase
    {
        private CompositeC1.Services.Facet.FacetConfigurationContext _target;

        // mocks
        private Mock<HttpContextBase> _httpContextMoq;
        private Mock<IDataQueryService> _dataQueryMoq;
        private Mock<ICacheStore<Guid, List<FacetSetting>>> _cacheMoq;

        // storage
        private List<IFacet> _facets;
        private List<IFacetConfiguration> _facetConfigs;
        private List<IPromotedFacetValueSetting> _promotedFacets;
        private List<IFacetConfigurationMeta> _facetsMeta;

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

            // cache
            var cacheServiceMoq = new Mock<ICacheService>();
            _cacheMoq = new Mock<ICacheStore<Guid, List<FacetSetting>>>();
            _cacheMoq
                .Setup(q => q.GetOrAdd(It.IsAny<Guid>(), It.IsAny<Func<Guid, List<FacetSetting>>>()))
                .Returns<Guid, Func<Guid, List<FacetSetting>>>((key, factory) => factory(key));
            cacheServiceMoq
                .Setup(q => q.GetStoreWithDependencies<Guid, List<FacetSetting>>(It.IsAny<string>(), It.IsAny<CacheDependentEntry[]>()))
                .Returns(_cacheMoq.Object);

            // storage
            _dataQueryMoq = new Mock<IDataQueryService>();

            _facets = new List<IFacet>();
            _dataQueryMoq.Setup(q => q.Get<IFacet>()).Returns(() => _facets.AsQueryable());

            _facetConfigs = new List<IFacetConfiguration>();
            _dataQueryMoq.Setup(q => q.Get<IFacetConfiguration>()).Returns(() => _facetConfigs.AsQueryable());

            _promotedFacets = new List<IPromotedFacetValueSetting>();
            _dataQueryMoq.Setup(q => q.Get<IPromotedFacetValueSetting>()).Returns(() => _promotedFacets.AsQueryable());

            _facetsMeta = new List<IFacetConfigurationMeta>();
            _dataQueryMoq.Setup(q => q.Get<IFacetConfigurationMeta>()).Returns(() => _facetsMeta.AsQueryable());

            // test target
            _target = new CompositeC1.Services.Facet.FacetConfigurationContext(_httpContextMoq.Object, _dataQueryMoq.Object, cacheServiceMoq.Object);
        }

        [Test]
        public void WHEN_no_facets_SHOULD_return_no_facetSettings()
        {
            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(0);
        }

        [Test]
        public void WHEN_no_default_facetSettings_SHOULD_return_any_facetSettings()
        {
            // arrange
            var facet = CreateFacet();
            var facetConfig = CreateFacetConfig(facet);
            facetConfig.IsDefault = false;

            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(1);
            facetSettings[0].FieldName.Should().Be(facet.FieldName);
        }

        [Test]
        public void WHEN_pageId_is_empty_SHOULD_return_default_facetSettings()
        {
            // arrange
            for (int i = 0; i < 10; i++)
            {
                var notDefaultFacet = CreateFacet();
                var notDefaultFacetConfig = CreateFacetConfig(notDefaultFacet);
                notDefaultFacetConfig.IsDefault = false;
            }

            var defaultFacet = CreateFacet();
            var defaultFacetConfig = CreateFacetConfig(defaultFacet);
            defaultFacetConfig.IsDefault = true;

            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(1);
            facetSettings[0].FieldName.Should().Be(defaultFacet.FieldName);
        }

        [Test]
        public void WHEN_pageId_is_given_SHOULD_return_corresponding_facetSettings()
        {
            // arrange
            _pageId = Guid.NewGuid();

            for (int i = 0; i < 10; i++)
            {
                var facet = CreateFacet();
                CreateConfigMeta(Guid.NewGuid(), CreateFacetConfig(facet));
            }

            var expectedFacet = CreateFacet();
            var expectedFacetConfig = CreateFacetConfig(expectedFacet);

            CreateConfigMeta(_pageId, expectedFacetConfig);

            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(1);
            facetSettings[0].FieldName.Should().Be(expectedFacet.FieldName);
        }


        [Test]
        public void WHEN_page_already_loaded_in_cache_SHOULD_return_facets_from_cache()
        {
            // arrange
            _pageId = Guid.NewGuid();
            var cachedFacetSettings = new List<FacetSetting>
            {
                new FacetSetting("Field1"),
            };
            _cacheMoq
                .Setup(q => q.GetOrAdd(It.IsAny<Guid>(), It.IsAny<Func<Guid, List<FacetSetting>>>()))
                .Returns<Guid, Func<Guid, List<FacetSetting>>>((key, factory) => cachedFacetSettings);

            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(1);
            facetSettings[0].FieldName.Should().Be(cachedFacetSettings[0].FieldName);
        }

        [Test]
        public void WHEN_loading_second_time_SHOULD_return_stored_facets()
        {
            // arrange
            var facet = CreateFacet();
            CreateFacetConfig(facet);
            _target.GetFacetSettings();
            _facets.Clear();
            _facetConfigs.Clear();
            
            // act
            var facetSettings = _target.GetFacetSettings();

            // assert
            facetSettings.Should().HaveCount(1);
            facetSettings[0].FieldName.Should().Be(facet.FieldName);
        }


        protected override FacetImpl CreateFacet(params FacetImpl[] facets)
        {
            var result = base.CreateFacet(facets);
            _facets.Add(result);
            return result;
        }

        protected override FacetConfigurationImpl CreateFacetConfig(params FacetImpl[] facets)
        {
            var result = base.CreateFacetConfig(facets);
            _facetConfigs.Add(result);
            return result;
        }

        protected override PromotedFacetValueSettingImpl CreatePromotedFacet()
        {
            var result = base.CreatePromotedFacet();
            _promotedFacets.Add(result);
            return result;
        }

        protected override FacetConfigurationMetaImpl CreateConfigMeta(Guid pageId, FacetConfigurationImpl configuration = null)
        {
            var result = base.CreateConfigMeta(pageId, configuration);
            _facetsMeta.Add(result);
            return result;
        }
    };
}
