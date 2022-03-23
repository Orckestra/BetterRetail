///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/ISearchRepository.ts' />

module Orckestra.Composer {

  export class RecipeDetailsController extends Controller {
    public VueRecipe: Vue;

    protected searchRepository: ISearchRepository = new SearchRepository();

    public initialize() {
      super.initialize();
      this.initializeVue();
    }

    public initializeVue() {
      const self = this;
      var ingredientsList = self.context.viewModel;
      this.VueRecipe = new Vue({
        el: '#vueRecipeDetails',
        data: {
          IngredientsList: ingredientsList

        },
        mounted() {

        },
        computed: {
        },
        methods: {
        }
      });
    }
  }
}
