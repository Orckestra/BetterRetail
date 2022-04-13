///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Composer.Product/ProductSearch/Services/MultiKeywordsSearchService.ts' />
///<reference path='../../Composer.Product/ProductSearch/Services/IMultiKeywordsSearchService.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Composer.Product/Product/ProductService.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />

module Orckestra.Composer {
  enum AddIngredientsToCartStates {
    Sucsess = 'sucsess',
    HasIssues = 'hasIssues'
  }

  export class RecipeDetailsController extends Controller {
    public VueRecipe: Vue;

    protected searchRepository: ISearchRepository = new SearchRepository();
    protected multiKeywordsSearchService: IMultiKeywordsSearchService = MultiKeywordsSearchService.instance();
    protected cartService: ICartService = CartService.getInstance();
    protected productService: ProductService = new ProductService(this.eventHub, this.context);
    protected inventoryService = new InventoryService();

    public initialize() {
      super.initialize();
      this.initializeVue();
    }

    public initializeVue() {
      const self = this;
      const SearchUrl = this.context.container.data('searchurl');
      var ingredientsList = self.context.viewModel;
      var ingredientsMap = ingredientsList.flatMap(a => a.Ingredients).reduce((map, obj) => { map[obj.Id] = obj; return map }, {});
      this.VueRecipe = new Vue({
        el: '#vueRecipeDetails',
        data: {
          IngredientsList: ingredientsList,
          IngredientsMap: ingredientsMap,
          ProductResultsMap: {},
          AvailableProductsSkus: [],
          SelectedIngredientsIds: [],
          AddToCartState: "",
          SearchUrl,
          Loading: false,
          PopoverIsInitialized: false,
        },
        mounted() {
          this.getIngredientsProducts();
        },
        updated() {
          if (!this.PopoverIsInitialized) {
            self.initializeRecipePopover();
            this.PopoverIsInitialized = true;
          }
        },
        computed: {
          SelectedKeywords() {
            return this.SelectedIngredients
            .map(a => a.Keyword)
            .filter((value, index, self) => value && self.indexOf(value) === index);
          },
          IsIngredientsSelected() {            
            return this.SelectedIngredientsIds.length > 0;
          },
          IsInredientKeywordsSelected() {
            return this.IsIngredientsSelected && this.SelectedKeywords.length > 0;
          },
          SelectedIngredients() {
            return this.SelectedIngredientsIds.map(id => this.IngredientsMap[id]);
          },
          SelectAll: {
            get: function () {
              return this.SelectedIngredientsIds.length ? this.SelectedIngredientsIds.length === Object.keys(this.IngredientsMap).length : false;
            },
            set: function (value) {
              this.SelectedIngredientsIds = value ? Object.keys(this.IngredientsMap) : [];
            }
          },
          AddToCartHasIssues() {
            return this.AddToCartState === AddIngredientsToCartStates.HasIssues;
          },
          AddToCartSucceeded() {
            return this.AddToCartState === AddIngredientsToCartStates.Sucsess;
          }
        },
        methods: {
          IsIngredientSelected(ingredientId) {
            return this.SelectedIngredientsIds.some(id => id === ingredientId)
          },
          IsProductAvailable(sku) {
            return this.AvailableProductsSkus.some(i => i === sku);
          },
          getIngredientsProducts() {
            const skuToFind = Object.keys(this.IngredientsMap).map(key => this.IngredientsMap[key].SKU).filter((item, index, self) => item && self.indexOf(item) == index);

            if (!_.isEmpty(skuToFind)) {
              let searchForProductsRequest = self.searchRepository.getProductsSearchResults("keywords=*", skuToFind);
              let productsAvailabilityRequest = self.inventoryService.getProductsAvailability(skuToFind);

              this.Loading = true;
              Q.all([searchForProductsRequest, productsAvailabilityRequest])
                .spread((productResults, availabilityResults) => {
                  this.ProductResultsMap = productResults.ProductSearchResults.SearchResults.reduce((map, obj) => { map[obj.Sku] = obj; return map; }, {});
                  this.AvailableProductsSkus = availabilityResults;
                }).then(n => this.addProductInfoToIngregient())
                .fin(() => this.Loading = false);
            }
          },
          addProductInfoToIngregient() {
            ingredientsList.forEach(list => {
              list.Ingredients.forEach(ingredient => {
                if (!ingredient.SKU) return;
                ingredient.ProductInformation = this.ProductResultsMap[ingredient.SKU];
                ingredient.CanBeAddedToCart = this.IsProductAvailable(ingredient.SKU);
              });
            });

            this.IngredientsList = { ...ingredientsList };
          },
          onImageError(e, item) {
            const img = item.ProductInformation.FallbackImageUrl;
            if (img) {
              e.target.onerror = null;
              e.target.src = img;
            }
          },
          addIngredientsToCart() {
            let promises = [];
            let availableIngredientsIds = [];

            this.SelectedIngredientsIds.forEach(id => {
              let ingredient = this.IngredientsMap[id];
              if (!ingredient.SKU) return;
              if (!this.IsProductAvailable(ingredient.SKU)) return;
              let productInfo = this.ProductResultsMap[ingredient.SKU];
              let promise: Q.Promise<any>;
              availableIngredientsIds.push(id);

              promise = self.cartService.addLineItem(productInfo.ProductId, productInfo.Price, null, 1);
              promise.fail(self.onAddToCartFailed);

              promises.push(promise);
            })

            let isFullyAdddedToCart = this.SelectedIngredientsIds.length === availableIngredientsIds.length;

            this.Loading = true;
            Q.all(promises).then(() => {
              this.AddToCartState = isFullyAdddedToCart ? AddIngredientsToCartStates.Sucsess : AddIngredientsToCartStates.HasIssues;
              this.SelectedIngredientsIds = this.SelectedIngredientsIds.filter((i) => (availableIngredientsIds.indexOf(i) === -1));
            }).fin(() => this.Loading = false)
          },
          searchIngredient() {
            if(!this.IsInredientKeywordsSelected) return;
            
            self.multiKeywordsSearchService.setKeywords(this.SelectedKeywords);
            window.location.href = this.SearchUrl + "?keywords=" + this.SelectedKeywords[0] + "&multikeywords=on";
          }
        }
      });
    }

    protected onAddToCartFailed(reason: any): void {
      console.error('Error on adding item to cart', reason);

      ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
    }

    protected initializeRecipePopover():void {
      this.context.container.find('.recipe-ingredient[data-toggle="popover"]').popover({
        html:true,
        content: function(){
          return document.getElementById(this.getAttribute('data-popover-content')).innerHTML;
        },
        trigger: 'hover',
        placement: 'top'
      });
    }
  }
}
