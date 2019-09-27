using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Data;
using FizzWare.NBuilder.Generators;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;

namespace Orckestra.Composer.CompositeC1.Tests.Services.FacetConfigurationContext
{
    public abstract class FacetTestsBase
    {
        #region Facet
        protected virtual FacetImpl CreateFacet(params FacetImpl[] dependsOn)
        {
            var facet = new FacetImpl
            {
                Id = Guid.NewGuid(),
                FieldName = GetRandom.String(10),
                FacetType = "SingleSelect",
                SortWeight = GetRandom.Decimal(),
                DependsOn = "",
                IsDisplayed = true,
                StartValue = "",
                EndValue = "",
                GapSize = "",
                MaxCollapsedValueCount = 5,
                MaxExpendedValueCount = null,
            };
            facet.DependsOnItems.AddRange(dependsOn);
            return facet;
        }

        protected class FacetImpl : IFacet
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
        protected virtual FacetConfigurationImpl CreateFacetConfig(params FacetImpl[] facets)
        {
            var config = new FacetConfigurationImpl
            {
                Id = Guid.NewGuid(),
                IsDefault = GetRandom.Boolean(),
                Name = GetRandom.String(10),
            };
            config.FacetsItems.AddRange(facets);
            return config;
        }

        protected class FacetConfigurationImpl : IFacetConfiguration
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
        protected virtual PromotedFacetValueSettingImpl CreatePromotedFacet()
        {
            var promoted = new PromotedFacetValueSettingImpl
            {
                Id = Guid.NewGuid(),
                SortWeight = GetRandom.Decimal(),
                Title = GetRandom.String(10),
            };

            return promoted;
        }

        protected class PromotedFacetValueSettingImpl : IPromotedFacetValueSetting
        {
            public DataSourceId DataSourceId { get; }
            public Guid Id { get; set; }
            public string Title { get; set; }
            public decimal SortWeight { get; set; }
        }
        #endregion

        #region Meta
        protected virtual FacetConfigurationMetaImpl CreateConfigMeta(Guid pageId, FacetConfigurationImpl configuration = null)
        {
            var meta = new FacetConfigurationMetaImpl
            {
                Id = Guid.NewGuid(),
                PageId = pageId,
                Configuration = configuration?.Id,
            };

            return meta;
        }

        protected class FacetConfigurationMetaImpl : IFacetConfigurationMeta
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
    };
}