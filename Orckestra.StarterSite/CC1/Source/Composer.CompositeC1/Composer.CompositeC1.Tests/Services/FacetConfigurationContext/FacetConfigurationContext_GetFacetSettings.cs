using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.CompositeC1.Services.DataQuery;

// ReSharper disable InconsistentNaming

namespace Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext
{
    [TestFixture]
    public class FacetConfigurationContext_GetFacetSettings
    {
        private CompositeC1.Services.FacetConfigurationContext _target;

        // mocks
        private Mock<HttpContextBase> _httpContextMoq;
        private Mock<IDataQueryService> _dataQueryMoq;

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
            _target = new CompositeC1.Services.FacetConfigurationContext(_httpContextMoq.Object, _dataQueryMoq.Object);
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
        public void WHEN_pageId_is_give_SHOULD_return_corresponding_facetSettings()
        {
            // arrange
            _pageId = Guid.NewGuid();

            for (int i = 0; i < 10; i++)
            {
                var facet = CreateFacet();
                CreateFacetConfig(facet);
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

        #region Helpers

        #region Facet
        private FacetImpl CreateFacet(params FacetImpl[] facets)
        {
            var facet = new FacetImpl
            {
                Id = Guid.NewGuid(),
                FieldName = RandomString(),
                FacetType = "SingleSelect",
                SortWeight = RandomDecimal(),
                DependsOn = "",
                IsDisplayed = true,
                StartValue = "",
                EndValue = "",
                GapSize = "",
                MaxCollapsedValueCount = 5,
                MaxExpendedValueCount = null,
            };
            facet.DependsOnItems.AddRange(facets);
            _facets.Add(facet);
            return facet;
        }

        private class FacetImpl : IFacet
        {
            public DataSourceId DataSourceId { get; }
            public Guid Id { get; set; }
            public string FieldName { get; set; }
            public string FacetType { get; set; }
            public decimal SortWeight { get; set; }
            public int MaxCollapsedValueCount { get; set; }
            public int? MaxExpendedValueCount { get; set; }

            public string DependsOn
            {
                get { return string.Join(",", DependsOnItems.Select(x => x.Id)); }
                set { }
            }

            public List<FacetImpl> DependsOnItems { get; } = new List<FacetImpl>();

            public string StartValue { get; set; }
            public string EndValue { get; set; }
            public string GapSize { get; set; }
            public bool IsDisplayed { get; set; }

            public string PromotedValues
            {
                get { return string.Join(",", PromotedValuesItems.Select(x => x.Id)); }
                set { }
            }

            public List<PromotedFacetValueSettingImpl> PromotedValuesItems { get; } = new List<PromotedFacetValueSettingImpl>();
        }
        #endregion

        #region FacetConfig
        private FacetConfigurationImpl CreateFacetConfig(params FacetImpl[] facets)
        {
            var config = new FacetConfigurationImpl
            {
                Id = Guid.NewGuid(),
                IsDefault = RandomBool(),
                Name = RandomString(),
            };
            config.FacetsItems.AddRange(facets);
            _facetConfigs.Add(config);
            return config;
        }

        private class FacetConfigurationImpl : IFacetConfiguration
        {
            public DataSourceId DataSourceId { get; }
            public Guid Id { get; set; }
            public string Name { get; set; }

            public string Facets
            {
                get { return string.Join(",", FacetsItems.Select(x => x.Id)); }
                set { }
            }

            public List<FacetImpl> FacetsItems { get; } = new List<FacetImpl>();

            public bool IsDefault { get; set; }
        }
        #endregion

        #region PromotedFacet
        private PromotedFacetValueSettingImpl CreatePromotedFacet()
        {
            var promoted = new PromotedFacetValueSettingImpl
            {
                Id = Guid.NewGuid(),
                SortWeight = RandomDecimal(),
                Title = RandomString(),
            };

            _promotedFacets.Add(promoted);
            return promoted;
        }

        private class PromotedFacetValueSettingImpl : IPromotedFacetValueSetting
        {
            public DataSourceId DataSourceId { get; }
            public Guid Id { get; set; }
            public string Title { get; set; }
            public decimal SortWeight { get; set; }
        }
        #endregion

        #region Meta
        private FacetConfigurationMetaImpl CreateConfigMeta(Guid pageId, FacetConfigurationImpl configuration = null)
        {
            var meta = new FacetConfigurationMetaImpl
            {
                Id = Guid.NewGuid(),
                PageId = pageId,
                Configuration = configuration?.Id,
            };

            _facetsMeta.Add(meta);
            return meta;
        }

        public class FacetConfigurationMetaImpl : IFacetConfigurationMeta
        {
            public Guid PageId { get; set; }
            public DataSourceId DataSourceId { get; }
            public Guid Id { get; set; }

            public string PublicationStatus { get; set; }
            public Guid VersionId { get; set; }
            public string FieldName { get; set; }
            public Guid? Configuration { get; set; }
        }
        #endregion

        #region Random
        private readonly Random _random = new Random();

        public string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public bool RandomBool()
        {
            return _random.Next() % 2 == 1;
        }

        public decimal RandomDecimal()
        {
            return (decimal)((_random.NextDouble() - 0.5) * 10.0);
        }
        #endregion

        #endregion
    };
}
