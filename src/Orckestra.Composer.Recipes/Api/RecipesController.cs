using Orckestra.Composer.Providers;
using Orckestra.Composer.Recipes.Parameters;
using Orckestra.Composer.Recipes.Services;
using Orckestra.Composer.Search;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Orckestra.Composer.Search.RequestConstants;

namespace Orckestra.Composer.Recipes.Api
{
    [ValidateLanguage]
    public class RecipesController : ApiController
    {
        protected IComposerContext ComposerContext { get; }
        protected IRecipesViewService RecipesViewService { get; }
        protected IScopeProvider ScopeProvider { get; set; }

        public RecipesController(IComposerContext composerContext, IRecipesViewService recipesViewService, IScopeProvider scopeProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            RecipesViewService = recipesViewService ?? throw new ArgumentNullException(nameof(recipesViewService));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        [HttpGet]
        [ActionName("favorites")]
        public virtual async Task<IHttpActionResult> GetFavorites()
        {
            var param = new GetCustomerRecipeFavoritesParam()
            {
                Scope = ScopeProvider.DefaultScope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                IsAuthenticated = ComposerContext.IsAuthenticated
            };

            var viewModel = await RecipesViewService.GetCustomerRecipeFavoritesViewModel(param).ConfigureAwait(false);
            
            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("addfavorite")]
        public virtual async Task<IHttpActionResult> AddFavorite([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            var param = new UpdateFavoriteParam()
            {
                EntityTypeName = Constants.CustomProfileRecipeFavorites,
                AttributeName = Constants.CustomerProfileRecipeFavorites,
                CustomerId = ComposerContext.CustomerId,
                Id = Guid.Parse(id),
                ScopeId = ScopeProvider.DefaultScope
            };

            await RecipesViewService.AddFavorite(param).ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete]
        [ActionName("removefavorite")]
        public virtual async Task<IHttpActionResult> RemoveFavorite([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

            var param = new UpdateFavoriteParam()
            {
                CustomerId = ComposerContext.CustomerId,
                ScopeId = ScopeProvider.DefaultScope,
                CultureInfo = ComposerContext.CultureInfo,
                EntityTypeName = Constants.CustomProfileRecipeFavorites,
                AttributeName = Constants.CustomerProfileRecipeFavorites,
                Id = Guid.Parse(id)
            };

            await RecipesViewService.RemoveFavorite(param).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        [ActionName("myfavorites")]
        public virtual async Task<IHttpActionResult> MyFavorites([FromBody] string query)
        {
            var queryString = HttpUtility.ParseQueryString(query ?? "");
            var CurrentPage = int.TryParse(queryString[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            var SortDirection = queryString[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
            var SortBy = queryString[SearchRequestParams.SortBy] ?? SearchRequestParams.DefaultSortBy;
            var Keywords = queryString[SearchRequestParams.Keywords];

            var recipeSearchVm = await RecipesViewService.GetCustomerRecipeFavoriteSearchResultsViewModel(new GetCustomerRecipeFavoriteSearchResultsParam()
            {
                Scope = ScopeProvider.DefaultScope,
                CustomerId = ComposerContext.CustomerId,
                Culture = ComposerContext.CultureInfo,
                PageSize = SearchConfiguration.MaxItemsPerPage,
                CurrentPage = CurrentPage,
                SearchQuery = Keywords ?? "*",
                QueryKeys = queryString?.AllKeys,
                SortBy = SortBy,
                SortDirection = SortDirection
            }).ConfigureAwait(false);

            return Ok(recipeSearchVm);
        }
    }
}
