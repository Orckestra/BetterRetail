using Composite.Core.Threading;
using Composite.Data;
using Composite.Search;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.ContentSearch.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Recipes.Parameters;
using Orckestra.Composer.Recipes.ViewModels;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Customers.CustomProfiles;
using Orckestra.Overture.ServiceModel.Requests.Customers.CustomProfiles;
using Orckestra.Search.WebsiteSearch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Recipes.Services
{
    public class RecipesViewService : IRecipesViewService
    {
        protected IComposerContext ComposerContext { get; }
        protected ICustomerRepository CustomerRepository { get; }
        protected ICustomProfilesRepository CustomProfileRepository { get; }
        protected IPageService PageService { get; }
        protected ISiteConfiguration SiteConfiguration { get; }
        protected IWebsiteContext WebsiteContext { get; }
        protected IContentSearchViewService ContentSearchViewService { get; private set; }

        public RecipesViewService(IComposerContext composerContext,
            ICustomerRepository customerRepository,
            ICustomProfilesRepository customProfileRepository,
            IPageService pageService,
            ISiteConfiguration siteConfiguration,
            IWebsiteContext websiteContext,
            IContentSearchViewService contentSearchViewService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            CustomProfileRepository = customProfileRepository ?? throw new ArgumentNullException(nameof(customProfileRepository));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            ContentSearchViewService = contentSearchViewService ?? throw new ArgumentNullException(nameof(contentSearchViewService));
        }

        public virtual List<IngredientsListViewModel> GetIngedientsListsViewModel(Guid recipeId)
        {
            var ingredientsList = DataFacade.GetData<IIngredientsList>().Where(l => l.Recipe == recipeId).OrderBy(l => l.Order).ToList();
            var listWithIngredients = new List<IngredientsListViewModel>();
            foreach (var iList in ingredientsList)
            {
                var ingredients = DataFacade.GetData<IIngredient>().Where(i => i.IngredientsList == iList.Id)
                    .OrderBy(i => i.Order)
                    .ToList();

                var isIgredientsPresent = ingredients.Count() > 0;
                if (!isIgredientsPresent) { continue; }
                var listNew = new IngredientsListViewModel()
                {
                    Id = iList.Id,
                    Title = iList.Title,
                    HideTitle = iList.HideTitle,
                    Ingredients = ingredients
                };

                listWithIngredients.Add(listNew);

            };
            return listWithIngredients;
        }

        public virtual async Task<RecipeSearchResultsViewModel> GetCustomerRecipeFavoriteSearchResultsViewModel(GetCustomerRecipeFavoriteSearchResultsParam param)
        {
            var customerRecipeFavorites = await GetCustomerRecipeFavorites(param.Scope, param.CustomerId, param.Culture).ConfigureAwait(false);
            
            if (!customerRecipeFavorites.Any())
            {
                return new RecipeSearchResultsViewModel();
            }

            param.FavoriteIds = customerRecipeFavorites.Select(item => item.ToString()).ToList();

            var searchQuery = param.SearchQuery?.Trim().ToLower();
            var searchRequest = GetSearchRequestForRecipeFavorites(param, searchQuery);
            var result = WebsiteSearchFacade.Search(searchRequest);

            using (var conn = new DataConnection(param.Culture))
            {
                var searchResult = new RecipeSearchResultsViewModel()
                {
                    PagesCount = (int)Math.Ceiling((decimal)result.ResultsFound / param.PageSize),
                    Total = result.ResultsFound,
                    SearchResults = result.Entries.Select(x => ContentSearchViewService.GetSearchResultsEntryViewModel(x)).ToList(),
                };
                return searchResult;

            }
        }
        
        protected virtual WebsiteSearchQuery GetSearchRequestForRecipeFavorites(GetCustomerRecipeFavoriteSearchResultsParam param, string searchQuery)
        {
            // keywords
            var sq = searchQuery == "*" ? "*:*" : searchQuery;
            string[] keywords = sq == null ? null : sq?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // facets
            var facets = new List<WebsiteSearchQueryFacet>();
            if (param.FavoriteIds != null && param.FavoriteIds.Any())
            {
                facets.Add(
                    new WebsiteSearchQueryFacet()
                    {
                        Name = "IRecipe.Id",
                        Selections = param.FavoriteIds.ToArray()
                    });
            }

            var sortOptions = new List<SearchQuerySortOption>();
            if (!string.IsNullOrEmpty(param.SortDirection) && !string.IsNullOrEmpty(param.SortBy))
            {
                var isReverted = param.SortDirection == "desc" ? true : false;
                sortOptions.Add(new SearchQuerySortOption(param.SortBy, isReverted, SortTermsAs.String));
            }

            return new WebsiteSearchQuery
            {
                Culture = param.Culture,
                CurrentSiteOnly = true,
                Keywords = keywords,
                DataTypes = new [] { typeof(IRecipe) },
                PageNumber = param.CurrentPage - 1,
                Facets = facets.ToArray(),
                PageSize = param.PageSize,
                SortOptions = sortOptions
            };
        }

        public virtual async Task<FavoritesViewModel> GetCustomerRecipeFavoritesViewModel(GetCustomerRecipeFavoritesParam param)
        {
            var signInUrl = GetSignInUrl(param.CultureInfo);
            if (!param.IsAuthenticated)
            {
                return new FavoritesViewModel()
                {
                    SignInUrl = signInUrl
                };
            }

            var customerRecipeFavorites = await GetCustomerRecipeFavorites(param.Scope, param.CustomerId, param.CultureInfo).ConfigureAwait(false);

            var viewModel = new FavoritesViewModel()
            {
                SignInUrl = signInUrl,
                FavoriteIds = customerRecipeFavorites
            };
            return viewModel;
        }

        private async Task<List<Guid>> GetCustomerRecipeFavorites(string scope, Guid customerId, CultureInfo cultureInfo)
        {
            var customerParam = new GetCustomerByIdParam()
            {
                Scope = scope,
                CustomerId = customerId,
                CultureInfo = cultureInfo
            };

            var customer = await CustomerRepository.GetCustomerByIdAsync(customerParam).ConfigureAwait(false);
            if (customer == null)
            {
                throw new ArgumentNullException("Customer does not exist");
            }

            if (!customer.PropertyBag.ContainsKey(Constants.CustomerProfileRecipeFavorites))
            {
                return new List<Guid>();
            }

            var favoriteIds = ((IEnumerable<Guid>)customer.PropertyBag[Constants.CustomerProfileRecipeFavorites]).ToList();
            return favoriteIds;
        }

        public virtual Task<List<CustomProfile>> GetProfileInstances(GetCustomProfilesParam param)
        {
            return CustomProfileRepository.GetProfileInstances(param);
        }

        public virtual async Task AddFavorite(UpdateFavoriteParam param)
        {
            var existingProfiles = await GetProfileInstances(new GetCustomProfilesParam()
            {
                Scope = param.ScopeId,
                EntityTypeName = param.EntityTypeName,
                CustomProfileIds = new List<Guid> { param.Id }
            });

            var request = new BulkUpdateProfilesRequest()
            {
                ScopeId = param.ScopeId,
                ProfileOperations = new ProfileOperations()
                {
                    AssociationsToAdd = new List<AssociationDescriptor>()
                    {
                        new AssociationDescriptor()
                        {
                            Id = param.Id,
                            AttributeName = param.AttributeName,
                            ParentEntityTypeName = Constants.Customer,
                            ParentId = param.CustomerId
                        }
                    }
                }
            };

            if (!existingProfiles.Any())
            {
                request.ProfileOperations.ProfilesToAdd = new List<CustomProfile>()
                {
                    new CustomProfile()
                    {
                        Id = param.Id,
                        EntityTypeName = param.EntityTypeName,
                        Name = param.Id.ToString(),
                        IsActive = true
                    }
                };
            }

            await CustomProfileRepository.BulkUpdateProfiles(request).ConfigureAwait(false);
        }

        public virtual async Task RemoveFavorite(UpdateFavoriteParam param)
        {
            var request = new BulkUpdateProfilesRequest()
            {
                ScopeId = param.ScopeId,
                ProfileOperations = new ProfileOperations()
                {
                    AssociationsToRemove = new List<AssociationDescriptor>()
                    {
                        new AssociationDescriptor()
                        {
                            Id = param.Id,
                            AttributeName = param.AttributeName,
                            ParentEntityTypeName = Constants.Customer,
                            ParentId = param.CustomerId
                        }
                    }
                }
            };

            await CustomProfileRepository.BulkUpdateProfiles(request).ConfigureAwait(false);
        }

        public virtual string GetSignInUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            using (ThreadDataManager.EnsureInitialize())
            {
                var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
                var signInPath = PageService.GetPageUrl(pagesConfiguration.LoginPageId, cultureInfo);

                return signInPath;
            }
        }
    }
}