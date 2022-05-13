///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Composer.Grocery/Recipes/Sevices/IRecipeFavoritesService.ts' />
///<reference path='../../Composer.Grocery/Recipes/Sevices/RecipeFavoritesService.ts' />
///<reference path='../../Repositories/RecipeFavoritesRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />

module Orckestra.Composer {
  enum AddIngredientsToCartStates {
    Sucsess = 'sucsess',
    HasIssues = 'hasIssues'
  }

  export class RecipeListController extends Controller {
    public VueRecipe: Vue;

    protected recipeFavoritesService: IRecipeFavoritesService = new RecipeFavoritesService(new RecipeFavoritesRepository());
    protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

    public initialize() {
      super.initialize();

      let authenticatedPromise = this.membershipService.isAuthenticated();
      let favPromise = this.recipeFavoritesService.getRecipeFavoritesSummary();
      Q.all([authenticatedPromise, favPromise ]).spread((authVm, {FavoriteIds}) => this.initializeVueComponent(authVm, FavoriteIds));
    }

    private initializeVueComponent(authVm, favorites) {
      const self = this;
      this.VueRecipe = new Vue({
        el: '#vueRecipeList',
        data: {
          FavoriteRecipes: favorites,
          IsAuthenticated: authVm.IsAuthenticated,
        },
        methods: {
          setFavorite(itemId, isFavorite) {
            if (!this.IsAuthenticated) {
              return self.recipeFavoritesService.redirectToSignIn();
            }

            if (isFavorite) {
              this.FavoriteRecipes = this.FavoriteRecipes.filter((obj => obj != itemId));
              self.recipeFavoritesService.removeFavorite(itemId);
            } else {
              this.FavoriteRecipes.push(itemId);
              self.recipeFavoritesService.addFavorite(itemId);
            }
          },
          IsFavorite(itemId) {
            return this.FavoriteRecipes ? this.FavoriteRecipes.indexOf(itemId) > -1 : false;
          }
        }
      })
    }
  }
}