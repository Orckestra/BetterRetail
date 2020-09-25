///<reference path='../Product/ProductController.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />

module Orckestra.Composer {

    enum RecurringMode  {
        Single = 'Single',
        Recurring = 'Recurring'
    }

    export class ProductDetailController extends Orckestra.Composer.ProductController {

        protected concern: string = 'productDetail';

        private selectedRecurringOrderFrequencyName: string;
        private recurringMode: RecurringMode;

        public initialize() {

            super.initialize();

            this.productService.updateSelectedKvasWith(this.context.viewModel.selectedKvas, this.concern);

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
            Q.when(this.renderData()).done(() => {
                addToCartBusy.done();
                let $recurringOrderContainer = this.context.container.find('[data-recurring-mode]');
                this.recurringMode = $recurringOrderContainer.data('recurring-mode');
                this.selectedRecurringOrderFrequencyName = $recurringOrderContainer.data('recurring-order-frequency');
            });
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
            this.eventHub.publish('productDetailsRendered', { data });
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {

            let varId = e.data.selectedVariantId || 'unavailable';
            var all = $('[data-variant]');

            $.each(all, (index, el) => {
                let $el = $(el);
                var vIds = $el.data('variant').toString().split(',');
                if (vIds.indexOf(varId) >= 0) {
                    this.handleHiddenImages($el);
                    $el.removeClass('d-none');
                } else {
                    $el.addClass('d-none');
                }
            });

            this.renderData().done();
        }

        protected handleHiddenImages(el) {
            el.find('img').each((index, img) => {
                if (!$(img).attr('src')) {
                    $(img).attr('src', $(img).data('src'));
                }
            });
        }

        protected onSelectedKvasChanged(e: IEventInformation) {

            this.render('KvaItems', {KeyVariantAttributeItems: e.data});
        }

        protected onPricesChanged(e: IEventInformation) {
            let vm = this.isProductWithVariants() && this.isSelectedVariantUnavailable() ? null : e.data;
            this.render('PriceDiscount', vm);
        }

        protected renderUnavailableAddToCart(): Q.Promise<void> {

            return Q.fcall(() => this.render('AddToCartProductDetail', {IsUnavailable: true}));
        }

        protected renderAddToCartButton(isAuthenticated: boolean) {
            let { Sku } = this.context.viewModel;

            if (!isAuthenticated && this.recurringMode === RecurringMode.Recurring) {
                return this.renderUnavailableAddToCart();
            }

            return this.inventoryService.isAvailableToSell(Sku)
                .then(IsAvailableToSell =>
                    this.render('AddToCartProductDetail', { IsAvailableToSell })
                );
        }

        protected renderAvailableAddToCart(): Q.Promise<void> {
            return this._membershipService.isAuthenticated()
                .then(({IsAuthenticated}) => this.renderAddToCartButton(IsAuthenticated));
        }

        public selectKva(actionContext: IControllerActionContext) {

            let currentSelectedVariantId = this.context.viewModel.selectedVariantId;

            super.selectKva(actionContext);

            //IE8 check
            if (history) {
                this.replaceHistory(currentSelectedVariantId);
            }
        }

        private replaceHistory(previousSelectedVariantId: string) {
            let variantId = this.context.viewModel.selectedVariantId;

            if (variantId === null && previousSelectedVariantId === null) {
                return;
            }

            let pathArray = window.location.pathname.split('/').filter(Boolean);

            let prevVariantIdIndex = pathArray.lastIndexOf(previousSelectedVariantId); //Variant id should be at the foremost right
            if (variantId === null) {
                if (prevVariantIdIndex !== -1) {
                    pathArray.splice(prevVariantIdIndex, 1);
                }
            } else  if (prevVariantIdIndex === -1) {
                //We couldn't find the variant id in the path, which means the PDP was accessed without a variant in the URL.
                //In that case, we add it right after the product id in the URL. If for some aweful reason the product id is not found,
                //add the variant id at the end.
                let productIdIndex = pathArray.indexOf(this.context.viewModel.productId);
                pathArray.splice(productIdIndex === -1 ? pathArray.length : productIdIndex + 1, 0, variantId);
            } else {
                //Replace the old variant id with the new one
                pathArray[prevVariantIdIndex] = variantId;
            }

            let {protocol, host} = window.location;
            let builtPath = `${protocol}//${host}${this.productService.buildUrlPath(pathArray)}`;

            history.replaceState( {} , null, builtPath);
        }

        protected completeAddLineItem(quantityAdded: any): Q.Promise<void> {

            var quantity = {
                Min: quantityAdded.Min,
                Max: quantityAdded.Max,
                Value: quantityAdded.Min
            };

            return this.renderAvailableQuantity(quantity);
        }

        public onRecurringOrderFrequencySelectChanged(actionContext: IControllerActionContext) {
            let element = <any>actionContext.elementContext[0],
                option = element.options[element.selectedIndex];

            if (option) {
                this.selectedRecurringOrderFrequencyName = option.value === '' ? null : option.value;
            }
        }

        public changeRecurringMode(actionContext: IControllerActionContext) {
            let container$ = actionContext.elementContext.closest('.js-recurringModes');
            container$.find('.js-recurringModeRow.selected').removeClass('selected');
            actionContext.elementContext.closest('.js-recurringModeRow').addClass('selected');
            $('.modeSelection').collapse('toggle');
            this.recurringMode = actionContext.elementContext.val();

            this._membershipService.isAuthenticated().then(({IsAuthenticated}) => {
                if (IsAuthenticated) { return; }
                return this.renderAddToCartButton(IsAuthenticated);
            });
        }

        public addToCartButtonClick(actionContext: IControllerActionContext) {
            let frequencyName = this.recurringMode === RecurringMode.Single ? null : this.selectedRecurringOrderFrequencyName;
            this.addLineItem(actionContext, frequencyName, this.context.viewModel.RecurringOrderProgramName);
        }
    }
}
