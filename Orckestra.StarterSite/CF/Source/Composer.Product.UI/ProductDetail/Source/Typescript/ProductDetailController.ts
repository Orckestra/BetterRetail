///<reference path='../../../Product/Source/Typescript/ProductController.ts' />

module Orckestra.Composer {
    export class ProductDetailController extends Orckestra.Composer.ProductController {

        protected _concern: string = 'productDetail';

        public initialize() {

            super.initialize();

            this.productService.updateSelectedKvasWith(this.context.viewModel.selectedKvas, this._concern);

            var priceDisplayBusy: UIBusyHandle = this.asyncBusy({
                msDelay: 300,
                loadingIndicatorSelector: '.loading-indicator-pricediscount'
            });

            var addToCartBusy: UIBusyHandle = this.asyncBusy({
                msDelay: 300,
                loadingIndicatorSelector: '.loading-indicator-inventory'
            });

            Q.when(this.calculatePrice()).done(() => {
                priceDisplayBusy.done();

                this.notifyAnalyticsOfProductDetailsImpression();
            });
            Q.when(this.renderData()).done(() => addToCartBusy.done());
        }

        protected getListNameForAnalytics(): string {
            return 'Detail';
        }

        protected notifyAnalyticsOfProductDetailsImpression() {
            var vm = this.context.viewModel;
            var variant: any = _.find(vm.allVariants, (v: any) => v.Id === vm.selectedVariantId);

            var data: any = this.getProductDataForAnalytics(vm);
            if (variant) {
                var variantData: any = this.getVariantDataForAnalytics(variant);

                _.extend(data, variantData);
            }

            this.publishProductImpressionEvent(data);
        }

        protected publishProductImpressionEvent(data: any) {
            this.eventHub.publish('productDetailsRendered',
                {
                    data: data
                });
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {

            this.renderData().done();
        }

        protected onSelectedKvasChanged(e: IEventInformation) {

            this.render('KvaItems', { KeyVariantAttributeItems: e.data });
        }

        protected onImagesChanged(e: IEventInformation) {

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {
                this.render('ProductImages', this.getUnavailableProductImages(e));
            } else {
                this.render('ProductImages', e.data);
            }
        }

        private getUnavailableProductImages(e: IEventInformation): any {

            var fallbackImageUrl: string = e.data.FallbackImageUrl;

            var image: any = {
                ThumbnailUrl: fallbackImageUrl,
                ImageUrl: fallbackImageUrl,
                Selected: true
            };

            var vm: any = {
                DisplayName: e.data.DisplayName,
                Images: [image],
                SelectedImage: {
                    ImageUrl: fallbackImageUrl
                }
            };

            return vm;
        }

        protected onPricesChanged(e: IEventInformation) {

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {
                this.render('PriceDiscount', null);
            } else {
                this.render('PriceDiscount', e.data);
            }
        }

        protected renderUnavailableAddToCart(): Q.Promise<void> {

            return Q.fcall(() => this.render('AddToCartProductDetail', { IsUnavailable: true }));
        }

        protected renderAvailableAddToCart(): Q.Promise<void> {

            var sku: string = this.context.viewModel.Sku;

            return this.inventoryService.isAvailableToSell(sku)
                       .then(result => this.render('AddToCartProductDetail', { IsAvailableToSell: result }));
        }

        public selectKva(actionContext: IControllerActionContext) {

            super.selectKva(actionContext);

            //IE8 check
            if (history) {
                this.productService.replaceHistory();
            }
        }

        protected completeAddLineItem(quantityAdded: any): Q.Promise<void> {

            var quantity = {
                Min: quantityAdded.Min,
                Max: quantityAdded.Max,
                Value: quantityAdded.Min
            };

            return this.renderAvailableQuantity(quantity);
        }
    }
}
