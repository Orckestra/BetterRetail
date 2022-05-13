using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;
using Composite.Search;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.ContentSearch.DataTypes;
using Orckestra.Composer.ContentSearch.Parameters;
using Orckestra.Composer.ContentSearch.ViewModels;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Search.WebsiteSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orckestra.Composer.ContentSearch.Services
{
    public class ContentSearchViewService : IContentSearchViewService
    {
        private readonly Dictionary<string, System.Type> KnowTypes = DataFacade.GetAllInterfaces().ToDictionary(t => t.FullName);
        protected IMediaService MediaService { get; private set; }
        private IWebsiteContext WebsiteContext { get; set; }
        public IPageService PageService { get; set; }
        public ISiteConfiguration SiteConfiguration { get; set; }

        public ContentSearchViewService(
            IMediaService mediaService, 
            IWebsiteContext websiteContext, 
            IPageService pageService, 
            ISiteConfiguration siteConfiguration)
        {
            MediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public virtual ContentSearchViewModel GetContentSearchViewModel(GetContentSearchParameter param)
        {
            using (var conn = new DataConnection(param.Culture))
            {
                var contentTabs = DataFacade.GetData<IContentTab>().Where(c => !string.IsNullOrEmpty(c.DataTypes)).OrderBy(t => t.Order).ToList();
                if (contentTabs == null || contentTabs.Count == 0) { return null; }

                var vm = new ContentSearchViewModel();

                foreach (var tab in contentTabs)
                {
                    var isActive = param.PathInfo == tab.UrlTitle;
                    var searchQuery = GetSearchQuery(param);
                    var searchRequest = GetSearchRequestForContentTab(param, tab, searchQuery);
                    var result = WebsiteSearchFacade.Search(searchRequest);

                    vm.Tabs.Add(new ContentSearchTabViewModel
                    {
                        Title = tab.Title,
                        UrlTitle = tab.UrlTitle,
                        TabUrl = GetTabUrl(param, tab, searchQuery),
                        PagesCount = (int)Math.Ceiling((decimal)result.ResultsFound / param.PageSize),
                        Total = result.ResultsFound,
                        IsActive = isActive,
                        SearchResults = result.Entries.Select(x => GetSearchResultsEntryViewModel(x)).ToList(),
                        DataTypes = tab.DataTypes
                    });

                    if(isActive)
                    {
                        vm.SelectedFacets = GetSelectedFacets(param, result);
                        vm.Facets = GetFacets(param, result);
                    }
                }

                var sortBys = DataFacade.GetData<ISortOption>().OrderBy(t => t.Order).ToList();
                vm.AvailableSortBys = sortBys;
                vm.SelectedSortBy = sortBys.Find(o => o.FieldName == param.SortBy && (o.IsReverted && param.SortDirection == "desc" || !o.IsReverted && param.SortDirection == "asc")) ?? sortBys.First();

                vm.SuggestedTabs = GetSuggestedTabs(param, contentTabs);

                return vm;
            }
        }

        public virtual SearchResultsEntryViewModel GetSearchResultsEntryViewModel(SearchResultEntry entry)
        {
            var vm = new SearchResultsEntryViewModel
            {
                Title = entry.Title,
                DetailsUrl = InternalUrls.TryConvertInternalUrlToPublic(entry.Url),
                ImageUrl = GetSearchEntryImage(entry),
                Description = GetSearchEntryDesc(entry),
                FieldsBag = entry.FieldValues
            };

            return vm;
        }

        protected virtual string GetSearchEntryImage(SearchResultEntry entry)
        {
            string imageResult = null, mimeTypeResult = null;

            foreach(var el in entry.FieldValues)
            {
                if (el.Key.Contains("Image"))
                {
                    imageResult = MediaService.GetMediaUrl(el.Value.ToString());
                    break;
                }
                //implementing the same logic, remembering the first mimetype, but keeping search for image key
                if (mimeTypeResult == null && el.Key.Contains("MimeType") && el.Value.ToString().StartsWith("image/"))
                {
                    mimeTypeResult = entry.Url;
                }
            }
            return imageResult ?? mimeTypeResult;
        }

        protected virtual string GetSearchEntryDesc(SearchResultEntry entry)
        {
            entry.FieldValues.TryGetValue("desc", out var desc);
            return desc?.ToString();
        }

        protected virtual List<FacetViewModel> GetSelectedFacets(GetContentSearchParameter param, WebsiteSearchResult searchResults)
        {
            var selectedFacets = new Dictionary<string, List<SearchResultFacetHit>>();
            int count = 0;
            foreach (var f in searchResults.Facets)
            {
                var facets = GetFacetSelection(param, f.Name).ToList();
                count += facets.Count;
                selectedFacets.Add(f.Name, f.Hits.Where(i => facets.Contains(i.Value)).ToList());
            }

            if (count > 0)
            {
                var vm = new List<FacetViewModel>();
                foreach (var f in selectedFacets)
                {
                    var facet = new FacetViewModel()
                    {
                        Label = f.Key,
                        Hits = new List<FacetHitViewModel>()
                    };

                    foreach (var hit in f.Value)
                    {
                        facet.Hits.Add(GetFacetHiViewModel(hit, f.Key));
                    }

                    vm.Add(facet);
                }
                return vm;
            }
            return null;
        }

        protected virtual List<FacetViewModel> GetFacets(GetContentSearchParameter param, WebsiteSearchResult searchResults)
        {
            var facets = new List<FacetViewModel>();
            var labels = WebsiteSearchFacade.GetFacetOptions().ToDictionary(t => t.Item1, t => t.Item2);

            foreach (var f in searchResults.Facets)
            {
                if(f.Hits.Count == 0) { continue;  }

                var facet = new FacetViewModel
                {
                    Label = labels[f.Name],
                    Hits = new List<FacetHitViewModel>()
                };

                foreach (var hit in f.Hits)
                {
                    facet.Hits.Add(GetFacetHiViewModel(hit, f.Name));
                }
                
                facets.Add(facet);
            }
            return facets;
        }

        protected virtual List<ContentSearchTabViewModel> GetSuggestedTabs(GetContentSearchParameter param, List<IContentTab> contentTabs)
        {
            List<ContentSearchTabViewModel> vm = null;
            if (param.ProductsTabActive && param.IsCorrectedSearchQuery)
            {
                vm = new List<ContentSearchTabViewModel>();
                foreach (var tab in contentTabs)
                {
                    var searchQuery = param.SearchQuery;
                    var searchRequest = GetSearchRequestForContentTab(param, tab, searchQuery);
                    var result = WebsiteSearchFacade.Search(searchRequest);

                    if (result.ResultsFound > 0)
                    {
                        vm.Add(new ContentSearchTabViewModel
                        {
                            Title = tab.Title,
                            UrlTitle = tab.UrlTitle,
                            TabUrl = GetTabUrl(param, tab, searchQuery),
                            Total = result.ResultsFound
                        });
                    }
                }
            }
            return vm;
        }
        protected virtual string GetSearchQuery(GetContentSearchParameter param)
        {
            return param.ProductsTabActive && param.IsCorrectedSearchQuery ?
                param.CorrectedSearchQuery :
                param.SearchQuery.Trim().ToLower();
        }

        protected virtual string GetTabUrl(GetContentSearchParameter param, IContentTab tab, string searchQuery)
        {
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.Culture, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.SearchPageId, param.Culture);
            if (url == null) return null;

            return $"{url}/{tab.UrlTitle}?{SearchRequestParams.Keywords}={HttpUtility.UrlEncode(searchQuery)}";
        }
       
        protected virtual WebsiteSearchQuery GetSearchRequestForContentTab(GetContentSearchParameter param, IContentTab tab, string searchQuery)
        {
            bool isTabActive = param.PathInfo == tab.UrlTitle;
            // keywords
            var sq = searchQuery == "*" ? "*:*" : searchQuery;
            string[] keywords = sq.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // datatypes
            var tabTypes = tab.DataTypes.Split(',').ToList();
            Type[] dataTypes = tabTypes?.Select(name => KnowTypes[name]).ToArray();

            // page types
            var tabPageTypes = tab.PageTypes?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // facets
            var tabFacetsToQuery = !string.IsNullOrEmpty(tab.Facets) ? tab.Facets.Split(',') : Array.Empty<string>();
            var facets = isTabActive ? tabFacetsToQuery.Select(fieldName => new WebsiteSearchQueryFacet
            {
                Name = fieldName,
                Selections = GetFacetSelection(param, fieldName)
            }).ToArray() : null;

            // media folders
            var selectedMediaFolders = tab.MediaFolders != null ? DataFacade.GetData<IMediaFileFolder>().Where(p => tab.MediaFolders.Contains(p.KeyPath))
            .Select(i => i.GetDataEntityToken()).ToArray() : null;

            var sortOptions = new List<SearchQuerySortOption>();
            if (isTabActive && !string.IsNullOrEmpty(param.SortDirection) && !string.IsNullOrEmpty(param.SortBy))
            {
                var isReverted = param.SortDirection == "desc" ? true : false;
                sortOptions.Add(new SearchQuerySortOption(param.SortBy, isReverted, SortTermsAs.String));
            }

            return new WebsiteSearchQuery
            {
                Culture = param.Culture,
                Keywords = keywords,
                DataTypes = dataTypes,
                PageTypes = tabPageTypes,
                CurrentSiteOnly = param.CurrentSiteOnly,
                PageNumber = isTabActive ? param.CurrentPage - 1 : 0,
                Facets = facets,
                PageSize = param.PageSize,
                SortOptions = sortOptions,
                FilterByAncestorEntityTokens = selectedMediaFolders
            };
        }
       
        protected virtual string[] GetFacetSelection(GetContentSearchParameter param, string fieldName)
        {
            var prefix = GetFacetFieldCheckboxPrefix(fieldName);
            return param.QueryKeys
                .Where(key => key.StartsWith(prefix))
                .Select(key => key.Substring(prefix.Length))
                .ToArray();
        }
       
        protected virtual string GetFacetFieldCheckboxPrefix(string fieldName)
        {
            return $"f_{fieldName}_";
        }
       
        protected virtual FacetHitViewModel GetFacetHiViewModel(SearchResultFacetHit hit, string facetName)
        {
            return new FacetHitViewModel
            {
                Label = hit.Label,
                Key = GetFacetFieldCheckboxPrefix(facetName) + hit.Value,
                Count = hit.HitCount
            };
        }
    }
}