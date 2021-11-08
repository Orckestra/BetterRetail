using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Factory
{
    public class FacetFactory : ProviderFactory<IFacetProvider>, IFacetFactory
    {
        protected IFacetProviderRegistry FacetProviderRegistry { get; }
        protected IFacetConfigurationContext FacetConfigContext { get; }

        public FacetFactory(IDependencyResolver dependencyResolver, IFacetProviderRegistry facetProviderRegistry, IFacetConfigurationContext facetConfigContext)
            : base(dependencyResolver)
        {
            FacetProviderRegistry = facetProviderRegistry ?? throw new ArgumentNullException(nameof(facetProviderRegistry));
            FacetConfigContext = facetConfigContext;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Facet"/> based on a <param name="facet"></param> object.
        /// </summary>
        public Facet CreateFacet(Overture.ServiceModel.Search.Facet facet, IReadOnlyList<SearchFilter> selectedFacets, CultureInfo cultureInfo)
        {
            if (facet == null) { throw new ArgumentNullException(nameof(facet)); }
            if (string.IsNullOrWhiteSpace(facet.FieldName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(facet.FieldName)), nameof(facet)); }

            var setting = FacetConfigContext.GetFacetSettings()
                    .Find(s => s.FieldName.Equals(facet.FieldName, StringComparison.OrdinalIgnoreCase));

            if (setting == null) { return null; }

            Type factoryType = FacetProviderRegistry.ResolveProviderType(setting.FacetType.ToString());

            var instance = GetProviderInstance(factoryType);

            return instance.CreateFacet(facet, setting, selectedFacets, cultureInfo);
        }

        public CategoryFacetValuesTree BuildCategoryFacetValuesTree(IList<Facet> facets,
            SelectedFacets selectedFacets,
            Tree<Overture.ServiceModel.Products.Category, string> categoryTree,
            CategoryFacetCounts counts,
            CultureInfo culture)
        {
            var categoryFacetSettings = FacetConfigContext.GetFacetSettings()
              .Where(s => s.IsCategoryFacet)?.ToList();
            if ((categoryFacetSettings?.Count ?? 0) == 0)
            {
                return null;
            }

            categoryTree.TryGetValue("Root", out TreeNode<Overture.ServiceModel.Products.Category> categoriesRoot);
            var tree = new CategoryFacetValuesTree() { TotalCount = counts.TotalCount };

            var rootFacetSetting = categoryFacetSettings.FirstOrDefault(c => (c.DependsOn?.Count ?? 0) == 0);
            if (rootFacetSetting != null)
            {
                tree.ChildNodes = GetTreeNodes(rootFacetSetting, facets, selectedFacets, categoriesRoot, culture, counts);
                tree.MaxCollapsedCount = rootFacetSetting.MaxCollapsedValueCount;
                tree.MaxExpandedCount = rootFacetSetting.MaxExpendedValueCount;
                BuildTreeNodes(categoryFacetSettings, rootFacetSetting.FieldName, facets, selectedFacets, tree.ChildNodes, categoryTree, counts, culture);
            }

            return tree;
        }

        protected void BuildTreeNodes(List<FacetSetting> categoryFacetSettings, 
            string nextFacetFieldName, 
            IList<Facet> facets, 
            SelectedFacets selectedFacets, 
            List<CategoryFacetValuesTreeNode> nodes,
            Tree<Overture.ServiceModel.Products.Category, string> categoryTree,
            CategoryFacetCounts counts,
            CultureInfo culture)
        {
            if((nodes?.Count ?? 0) == 0)
            {
                return;
            }

            var facetSetting = categoryFacetSettings.FirstOrDefault(c => c.DependsOn != null && c.DependsOn.Contains(nextFacetFieldName));
  
            if (facetSetting != null)
            {
                foreach (var node in nodes)
                {
                    categoryTree.TryGetValue(node.CategoryId, out TreeNode<Overture.ServiceModel.Products.Category> categoriesRoot);
                    node.ChildNodes  = GetTreeNodes(facetSetting, facets, selectedFacets, categoriesRoot, culture, counts);
                    node.MaxCollapsedCount = facetSetting.MaxCollapsedValueCount;
                    node.MaxExpandedCount = facetSetting.MaxExpendedValueCount;

                    BuildTreeNodes(categoryFacetSettings, facetSetting.FieldName, facets, selectedFacets, node.ChildNodes, categoryTree, counts, culture);
                }
            }
        }

        protected List<CategoryFacetValuesTreeNode> GetTreeNodes(FacetSetting facetSetting, IList<Facet> facets, SelectedFacets selectedFacets,
             TreeNode<Overture.ServiceModel.Products.Category> categoryTree, CultureInfo culture, CategoryFacetCounts counts)
        {
            var categoryChildren = categoryTree.Children;
            var categoryChildrenLookup = categoryChildren.ToLookup(_ => _.Value.DisplayName.GetLocalizedValue(culture.Name));
            var facet = facets.FirstOrDefault(f => f.FieldName == facetSetting.FieldName);
            var countsForFacetValues = counts?.Facets?.FirstOrDefault(fc => facetSetting.FieldName.StartsWith(fc.FieldName))?.FacetValues;

            List<CategoryFacetValuesTreeNode> nodes = null;

            var facetValues = facet?.FacetValues.Concat(facet.OnDemandFacetValues);
            if (facetValues != null)
            {
                nodes = (from fv in facetValues
                         let category = categoryChildrenLookup[fv.Value].FirstOrDefault()
                         where category != null
                         let totalCount = countsForFacetValues?.FirstOrDefault(fcv => fcv.Value.Equals(category.Value.Id, StringComparison.OrdinalIgnoreCase))?.Quantity
                         select new CategoryFacetValuesTreeNode(fv.Title, fv.Value, totalCount != null ? totalCount.Value : fv.Quantity, facetSetting.FacetType, facetSetting.FieldName, fv.IsSelected, true)
                         {
                             CategoryId = category.Value.Id
                         }).ToList();
            }

            if (nodes == null || nodes.Count == 0)
            {
                var selected = selectedFacets.Facets.Where(f => f.FieldName == facetSetting.FieldName);
                if (selected != null && selected.Count() > 0)
                {
                    nodes = (from fv in selected
                             let category = categoryChildrenLookup[fv.Value].FirstOrDefault()
                             where category != null
                             let totalCount = countsForFacetValues.FirstOrDefault(fcv => fcv.Value.Equals(category.Value.Id, StringComparison.OrdinalIgnoreCase))?.Quantity
                             select new CategoryFacetValuesTreeNode(fv.DisplayName, fv.Value, totalCount != null ? totalCount.Value : 0, facetSetting.FacetType, facetSetting.FieldName, true, fv.IsRemovable)
                             {
                                 CategoryId = category.Value.Id
                             }).ToList();
                }
            }

            return nodes?.OrderByDescending(n => n.Quantity).ToList();
        }
    }
}