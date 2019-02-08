/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/JqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/ErrorHandling/ErrorHandler.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerContext.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Repositories/CartRepository.ts' />
/// <reference path='../../../../Composer.Cart.UI/CartSummary/Source/TypeScript/CartService.ts' />
/// <reference path='../../../Product/Source/TypeScript/ProductService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: CartService = new CartService(new CartRepository(), this.eventHub);
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected currentPage: any;

        public initialize() {

            super.initialize();

            this.currentPage = this.getCurrentPage();

            this.eventHub.publish('searchResultRendered', {
                    data: {
                        ProductSearchResults: this.context.viewModel.SearchResults,
                        Keywords: this.context.viewModel.Keywords,
                        TotalCount: this.context.viewModel.TotalCount,
                        ListName: this.context.viewModel.ListName,
                        PageNumber: this.currentPage.DisplayName,
                        MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                    }
                }
            );
        }

        private getCurrentPage(): any {

            return <any>this.context.viewModel.PaginationCurrentPage;
        }

        public addToCart(actionContext: IControllerActionContext) {

            let productContext: JQuery = $(actionContext.elementContext).closest('[data-product-id]'),
                hasVariants: string = <any>productContext.data('hasVariants'),
                productId: string = productContext.attr('data-product-id'), //Do not use .data since it may parse the id as a number.
                variantId: string = productContext.attr('data-product-variant-id'),
                product = _.find(this.context.viewModel.SearchResults, function (product: any) {
                    if (_.isEmpty(variantId)) {
                        return product.ProductId === productId;
                    } else {
                        return product.ProductId === productId && product.VariantId === variantId;
                    }
                }),
                price: number = product.Pricing.IsOnSale ? product.Pricing.Price : product.Pricing.ListPrice,
                busy = this.asyncBusy({elementContext: actionContext.elementContext, containerContext: productContext});

            if (hasVariants === 'True') {

                this.productService.loadQuickBuyProduct(productId, variantId, 'productSearch', this.context.viewModel.ListName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());

            } else {
                let productData: any = this.getProductDataForAnalytics(productId, price);
                this.eventHub.publish('lineItemAdding', {data: productData});

                this.cartService.addLineItem(productId, '' + price)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());
            }
        }

        protected onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        public searchProductClick(actionContext: IControllerActionContext) {
            let index: number = <any>actionContext.elementContext.data('index'),
                productId: string = actionContext.elementContext.data('productid').toString(),
                product: any = _.find(this.context.viewModel.SearchResults, {ProductId: productId});

            this.eventHub.publish('productClick', {
                data: {
                    Product: product,
                    ListName: this.context.viewModel.ListName,
                    Index: index,
                    PageNumber: this.currentPage.DisplayName,
                    MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                }
            });
        }

        public pagerPageChanged(actionContext: IControllerActionContext) {
            this.context.window.location.href = actionContext.elementContext.val() as string;
        }

        protected getProductDataForAnalytics(productId: string, price: any): any {
            let results = this.context.viewModel.SearchResults,
                vm = _.find(results, (r: any) => r.ProductId === productId);

            if (!vm) {
                throw new Error(`Could not find a product with the ID '${productId}'.`);
            }

            let data = {
                List: this.context.viewModel.ListName,
                ProductId: vm.ProductId,
                DisplayName: vm.DisplayName,
                ListPrice: price,
                Brand: vm.Brand,
                CategoryId: vm.CategoryId,
                Quantity: 1
            };

            return data;
        }
    }
}
