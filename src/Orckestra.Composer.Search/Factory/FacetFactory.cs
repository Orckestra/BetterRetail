using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.Facet;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
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
            Tree<Overture.ServiceModel.Products.Category, string> categoriesTree,
            CategoryFacetCounts counts,
            CultureInfo culture)
        {
            var categoryTreeViewFacets = FacetConfigContext.GetFacetSettings()
              .Where(s => s.IsCategoryTreeViewFacet)?.ToList();
            if (categoryTreeViewFacets == null || categoryTreeViewFacets.Count == 0)
            {
                return null;
            }

            categoriesTree.TryGetValue("Root", out TreeNode<Overture.ServiceModel.Products.Category> categoriesRoot);
            var tree = new CategoryFacetValuesTree() { RootTotalCount = counts.TotalCount };

            var rootFacetSetting = categoryTreeViewFacets.FirstOrDefault(c => c.DependsOn == null || c.DependsOn.Count == 0);
            if (rootFacetSetting != null)
            {
                tree.Items = GetTreeItems(rootFacetSetting, facets, selectedFacets, categoriesRoot, culture, counts);
                BuildTreeItems(categoryTreeViewFacets, rootFacetSetting.FieldName, facets, selectedFacets, tree.Items, categoriesTree, counts, culture);
            }

            return tree;
        }

        protected void BuildTreeItems(List<FacetSetting> categoryFacetSettings, 
            string nextFieldName, IList<Facet> facets, 
            SelectedFacets selectedFacets, List<CategoryFacetValuesTreeItem> items,
            Tree<Overture.ServiceModel.Products.Category, string> categoriesTree,
            CategoryFacetCounts counts,
            CultureInfo culture)
        {
            if(items == null || items.Count == 0)
            {
                return;
            }

            var facetSetting = categoryFacetSettings.FirstOrDefault(c => c.DependsOn != null && c.DependsOn.Contains(nextFieldName));
  
            if (facetSetting != null)
            {
                foreach (var item in items)
                {
                    categoriesTree.TryGetValue(item.CategoryId, out TreeNode<Overture.ServiceModel.Products.Category> categoriesRoot);
                    var allItems = GetTreeItems(facetSetting, facets, selectedFacets, categoriesRoot, culture, counts);

                    item.Items = allItems.Take(facetSetting.MaxCollapsedValueCount).ToList();
                    item.OnDemandItems = allItems
                        .Skip(facetSetting.MaxCollapsedValueCount)
                        .Take(facetSetting.MaxExpendedValueCount - facetSetting.MaxCollapsedValueCount)
                        .ToList();

                    BuildTreeItems(categoryFacetSettings, facetSetting.FieldName, facets, selectedFacets, item.Items, categoriesTree, counts, culture);
                    BuildTreeItems(categoryFacetSettings, facetSetting.FieldName, facets, selectedFacets, item.OnDemandItems, categoriesTree, counts, culture);
                }
            }
        }
        
        protected List<CategoryFacetValuesTreeItem> GetTreeItems(FacetSetting facetSetting, IList<Facet> facets, SelectedFacets selectedFacets,
             TreeNode<Overture.ServiceModel.Products.Category> categoriesTree, CultureInfo culture, CategoryFacetCounts counts)
        {
            var categoryChildren = categoriesTree.Children;
            var facet = facets.FirstOrDefault(f => f.FieldName == facetSetting.FieldName);

            var facetValues = facet?.FacetValues.Concat(facet.OnDemandFacetValues);
            var items = facetValues?
                        .Where(fv => categoryChildren.FirstOrDefault(dn => dn.Value.DisplayName.GetLocalizedValue(culture.Name) == fv.Value) != null)
                        .OrderByDescending(fv => fv.Quantity)
                        .Select(fv =>
                        {
                            var item = new CategoryFacetValuesTreeItem(fv.Title, fv.Value, fv.Quantity, facetSetting.FacetType, facetSetting.FieldName, false, true)
                            {
                                CategoryId = categoryChildren.FirstOrDefault(dn => dn.Value.DisplayName.GetLocalizedValue(culture.Name) == fv.Value).Value.Id
                            };
                            return item;
                        })
                        .ToList();

            if (items == null || items.Count == 0)
            {
                items = selectedFacets.Facets.Where(f => f.FieldName == facetSetting.FieldName)
                 .Where(fv => categoryChildren.FirstOrDefault(c => c.Value.DisplayName.GetLocalizedValue(culture.Name) == fv.Value) != null)
                .Select(fv =>
                {
                    var catId = categoryChildren.FirstOrDefault(dn => dn.Value.DisplayName.GetLocalizedValue(culture.Name) == fv.Value).Value.Id;
                    var totalCount = counts.Facets.FirstOrDefault(fc => facetSetting.FieldName.StartsWith(fc.FieldName))?
                    .FacetValues.FirstOrDefault(fcv => fcv.Value.Equals(catId, StringComparison.OrdinalIgnoreCase))?.Quantity;
                    var item = new CategoryFacetValuesTreeItem(fv.DisplayName, fv.Value, totalCount != null ? totalCount.Value : 0, facetSetting.FacetType, facetSetting.FieldName, true, fv.IsRemovable)
                    {
                        CategoryId = catId
                    };
                    return item;
                }).ToList();
            }

            return items;
        }
    }
}