/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typings/vue/index.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Repositories/CartRepository.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../../Utils/PriceHelper.ts' />
///<reference path='../../Repositories/ISearchRepository.ts' />
///<reference path='../../Repositories/SearchRepository.ts' />
///<reference path='../../Composer.Grocery/FulfillmentEvents.ts' />
/// <reference path='../../Composer.Product/ProductSearch/Constants/SearchEvents.ts' />

module Orckestra.Composer {
  'use strict';

  export class MyUsualsController extends Orckestra.Composer.Controller {
    protected cartService: ICartService = CartService.getInstance();
    protected wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
    protected searchRepository: ISearchRepository = new SearchRepository();
    protected currentPage: any;
    protected VueMyUsualsResults: Vue;

    public initialize() {
      super.initialize();
      this.initializeVueComponent();
    }

    private initializeVueComponent() {
      let self = this;
      let inputField;
      const SearchUrl = this.context.container.data('searchurl');
      var { ProductSearchResults } = this.context.viewModel;

      this.VueMyUsualsResults = new Vue({
        el: "#vueMyUsuals",
        components: {},
        mounted() {
          self.eventHub.subscribe(CartEvents.CartUpdated, this.onCartUpdated);
          self.eventHub.subscribe(FulfillmentEvents.StoreSelected, this.onStoreSelected);
          self.eventHub.subscribe(SearchEvents.SearchRequested, this.onSearchRequested.bind(this));
          self.cartService.getCart()
            .then(cart => this.Cart = cart)
            .fin(() => this.IsBusy = false);

          self.wishListService.getWishListSummary()
            .then(wishList => this.WishList = wishList);

          inputField = document.getElementById('filter_keywords');
          if (inputField !== null) {
            inputField.addEventListener('keydown', (event) => {
              if (event.code === 'Enter') {
                this.filterMyUsuals();
              }
            });
          }

        },
        data: {
          ...ProductSearchResults,
          keywords: "*",
          Cart: undefined,
          WishList: undefined,
          UpdatingProductId: undefined,
          Loading: false,
          IsBusy: true,
          SearchUrl,
        },
        computed: {
          ExtendedSearchResults() {
            const results = _.map(this.SearchResults, (product: any) => {
              const isSameProduct = (i: any) => i.ProductId === product.ProductId;
              let cartItem = this.Cart && this.Cart.LineItemDetailViewModels.find(isSameProduct);
              product.InCart = !!cartItem;
              product.LineItemId = cartItem ? cartItem.Id : undefined;
              product.Quantity = cartItem ? cartItem.Quantity : 0;

              const wishListItem = this.WishList && this.WishList.Items.find(isSameProduct);
              product.InWishList = !!wishListItem;
              product.WishListItemId = wishListItem ? wishListItem.Id : undefined;

              product.PricePerUnit = PriceHelper.PricePerUnit(product.DisplayListPrice,
                product.ProductUnitQuantity,
                product.ProductUnitSize,
                product.ConvertedVolumeMeasurement
              );

              if (product.PricePerUnit) {
                product.IsPricePerUnitZero = PriceHelper.IsPricePerUnitZero(product.PricePerUnit);
              }
              return product;
            });
            return results;
          },
          UpdatingProduct() {
            return _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.ProductId == this.UpdatingProductId);
          }
        },
        methods: {
          updateKeyword(value) {
            this.keywords = value;
          },
          searchForMyUsuals(queryString: string) {
            this.Loading = true;
            self.searchRepository.getMyUsualsSearchResults(queryString).then(results => {
              this.SearchResults = results.ProductSearchResults.SearchResults;
              this.TotalCount = results.ProductSearchResults.TotalCount;
              this.SelectedSortBys = results.ProductSearchResults.SelectedSortBy;
              this.Pagination = results.ProductSearchResults.Pagination;

              Object.keys(results.ProductSearchResults).forEach(key => this[key] = results.ProductSearchResults[key]);
              self.eventHub.publish(SearchEvents.SearchResultsLoaded, { data: results });
            }).fin(() => this.Loading = false)
          },
          onCartUpdated(result) {
            this.Cart = result.data;
          },
          searchGrocery() {
            var searchForProductsUrl = `${this.SearchUrl}?keywords=${this.keywords}`
            window.location.href = searchForProductsUrl;
          },
          onStoreSelected() {
            this.SearchResults = this.searchForMyUsuals();
          },
          filterMyUsuals() {
            self.eventHub.publish(SearchEvents.FilterKeywordChanged, { data: { keyword: this.keywords } });
          },
          clearFilter() {
            inputField.value = "";
            this.keywords = "";
            self.eventHub.publish(SearchEvents.FilterKeywordChanged, { data: {} });
          },
          updateItemQuantity(item: any, quantity: number) {
            let cartItem = _.find(this.Cart.LineItemDetailViewModels, (i: any) => i.Id === item.LineItemId);

            if (this.Loading || !cartItem) return;

            if (this.Cart.QuantityRange) {
              const { Min, Max } = this.Cart.QuantityRange;
              quantity = Math.min(Math.max(Min, quantity), Max);
            }

            if (quantity == cartItem.Quantity) {
              //force update vue component
              this.Cart = { ...this.Cart };
              return;
            }

            cartItem.Quantity = quantity;

            if (cartItem.Quantity < 1) {
              this.Loading = true; // disabling UI immediately when a line item is removed
            }

            if (!this.debounceUpdateItem) {
              this.debounceUpdateItem = _.debounce(({ Id, Quantity, ProductId }) => {
                this.Loading = true;
                this.UpdatingProductId = ProductId;
                let updatePromise = Quantity > 0 ?
                  self.cartService.updateLineItem(Id, Quantity, ProductId) :
                  self.cartService.deleteLineItem(Id, ProductId);

                updatePromise
                  .then(() => {
                    ErrorHandler.instance().removeErrors();
                  }, (reason: any) => {
                    this.onAddToCartFailed(reason);
                    throw reason;
                  })
                  .fin(() => this.Loading = false);

              }, 400);
            }
            this.debounceUpdateItem(cartItem);
          },
          addLineItemToWishList(item, event: JQueryEventObject) {
            let { DisplayName, ProductId, VariantId, ListPrice, RecurringOrderProgramName } = item;
            self.eventHub.publish('wishListLineItemAdding', {
              data: { DisplayName, ListPrice: ListPrice }
            });
            self.wishListService.addLineItem(ProductId, VariantId, 1, null, RecurringOrderProgramName)
              .then(wishList => this.WishList = wishList).fail(this.onAddToWishFailed);
          },
          removeLineItemFromWishList(item, event: JQueryEventObject) {
            self.wishListService.removeLineItem(item.WishListItemId)
              .then(wishList => this.WishList = wishList).fail(this.onAddToWishFailed);
          },
          loadingProduct(product, loading) {
            this.Loading = loading;
            this.SearchResults = [...this.SearchResults];
          },
          addToCart(event: any, product: any): void {
            const {
              ProductId: productId,
              RecurringOrderProgramName: recurringOrderProgramName
            } = product;

            const price: number = product.IsOnSale ? product.Price : product.ListPrice;

            this.loadingProduct(product, true);

            self.cartService.addLineItem(productId, '' + price, null, 1, null, recurringOrderProgramName)
              .then(this.addToCartSuccess, this.onAddToCartFailed)
              .fin(() => this.loadingProduct(product, false));
          },

          onSearchRequested({ data }): void {
            this.searchForMyUsuals(data.queryString);
          },

          sortingChanged(url: string): void {
            self.eventHub.publish(SearchEvents.SortingChanged, { data: { url } });
          },

          onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
          },
          addToCartSuccess(data: any): void {
            ErrorHandler.instance().removeErrors();
            return data;
          },
          onAddToWishFailed(reason: any): void {
            console.error('Error on adding item to wishList', reason);
            self.wishListService.clearCache();
            ErrorHandler.instance().outputErrorFromCode('AddToWishListFailed');
          }
        }
      });
    }
  }
}
