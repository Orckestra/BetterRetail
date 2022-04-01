///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Composer.Product/ProductSearch/Services/MultiKeywordsSearchService.ts' />
///<reference path='../../Composer.Product/ProductSearch/Services/IMultiKeywordsSearchService.ts' />

module Orckestra.Composer {

  export class RecipeDetailsController extends Controller {
    public VueRecipe: Vue;

    protected searchRepository: ISearchRepository = new SearchRepository();
    protected multiKeywordsSearchService: IMultiKeywordsSearchService = MultiKeywordsSearchService.instance();

    public initialize() {
      super.initialize();
      this.initializeVue();
    }

    public initializeVue() {
      const self = this;
      const SearchUrl = this.context.container.data('searchurl');
      var ingredientsList = self.context.viewModel;
      this.VueRecipe = new Vue({
        el: '#vueRecipeDetails',
        data: {
          IngredientsList: ingredientsList,
          IngredientsMap: ingredientsList.flatMap(a => a.Ingredients).reduce((map, obj) => { map[obj.Id] = obj; return map }, {}),
          SearchUrl,
          SelectedIngredientsIds:[]
        },
        mounted() {      
        },
        computed: {
          IsIngredientsSelected(){
            return this.SelectedIngredientsIds.length > 0;
          },
          SelectedIngredients() {
            return this.SelectedIngredientsIds.map(id => this.IngredientsMap[id]);
          },
          SelectAll: {
            get: function () {
                return this.SelectedIngredientsIds.length ? this.SelectedIngredientsIds.length == this.IngredientsMap.size : false;
            },
            set: function (value) {
              this.SelectedIngredientsIds = value ? Object.keys(this.IngredientsMap) : [];
            }
        }
        },
        methods: {         
          searchIngredient() {
            var keywords = this.SelectedIngredients
              .map(a => a.Keyword)
              .filter((value, index, self) => self.indexOf(value) === index);

            self.multiKeywordsSearchService.setKeywords(keywords);
            window.location.href = this.SearchUrl + "?keywords=" + keywords[0] + "&multikeywords=on";
          }
        }
      });
    }
  }
}
