///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../../UI/UIBusyHandle.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='./InventoryService.ts' />
///<reference path='./ProductService.ts' />
///<reference path='./ProductFormatter.ts' />
///<reference path='./KeyVariantAttributeItemsBuilder.ts' />

module Orckestra.Composer {

    export class ProductController extends Orckestra.Composer.Controller {

        protected inventoryService = new InventoryService();
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected cartService: ICartService = CartService.getInstance();
        protected _wishListService: WishListService = new WishListService(new WishListRepository(), this.eventHub);
        protected _membershipService: IMembershipService = new MembershipService(new MembershipRepository());


        protected concern: string;

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(this.concern + 'DisplayedVariantIdChanged', e => this.onSelectedVariantIdChanged(e));
            this.eventHub.subscribe(this.concern + 'SelectedVariantIdChanged', e => this.onSelectedVariantIdChanged(e));
            this.eventHub.subscribe(this.concern + 'SelectedKvasChanged', e => this.onSelectedKvasChanged(e));
            this.eventHub.subscribe(this.concern + 'ImagesChanged', e => this.onImagesChanged(e));
            this.eventHub.subscribe(this.concern + 'PricesChanged', e => this.onPricesChanged(e));
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {

            return;
        }

        protected onSelectedKvasChanged(e: IEventInformation) {

            return;
        }

        protected onImagesChanged(e: IEventInformation) {

            return;
        }

        protected onPricesChanged(e: IEventInformation) {

            return;
        }

        protected renderData(): Q.Promise<void[]> {

            var quantity = this.getCurrentQuantity();
            var renderTasks: Array<Q.Promise<any>> = [];

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {

                renderTasks.push(this.renderUnavailableQuantity(quantity));
                renderTasks.push(this.renderUnavailableAddToCart());

            } else {

                renderTasks.push(this.renderAvailableQuantity(quantity));
                renderTasks.push(this.renderAvailableAddToCart());
            }
            renderTasks.push(this.renderAddToWishList());

            return Q.all(renderTasks);
        }

        protected isProductWithVariants(): boolean {

            return $.isArray(this.context.viewModel.allVariants);
        }

        protected isSelectedVariantUnavailable(): boolean {

            return !this.context.viewModel.selectedVariantId;
        }

        protected renderUnavailableQuantity(quantity: any): Q.Promise<void> {

            return Q.fcall(() => this.render('ProductQuantity', { Quantity: quantity, Disabled: true }));
        }

        protected renderAvailableQuantity(quantity: any): Q.Promise<void> {

            return this.inventoryService
                .isAvailableToSell(this.context.viewModel.Sku)
                .then(result => this.render('ProductQuantity', { Quantity: quantity, Disabled: !result }));
        }

        protected renderAddToWishList(): Q.Promise<void> {
            let vm = this.context.viewModel;
            this.render('AddToWishList', { Loaded: false });

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {
                return;
            }

            return this._wishListService.getLineItem(vm.productId, vm.selectedVariantId)
                .then(result => {
                    if (result) {
                        this.render('AddToWishList', { Loaded: true, IsInWishList: true, Id: result.Id });
                    } else {
                        this.render('AddToWishList', { Loaded: true, IsInWishList: false });
                    }
                });
        }

        protected renderUnavailableAddToCart(): Q.Promise<void> {

            return;
        }

        protected renderAvailableAddToCart(): Q.Promise<void> {

            return;
        }

        public decrementQuantity(actionContext: IControllerActionContext) {
            var quantity = this.getCurrentQuantity();
            quantity.Value--;

            actionContext.event.preventDefault();
            this.renderAvailableQuantity(quantity).done();
        }

        public incrementQuantity(actionContext: IControllerActionContext) {
            var quantity = this.getCurrentQuantity();
            quantity.Value++;

            actionContext.event.preventDefault();
            this.renderAvailableQuantity(quantity).done();
        }

        public changeQuantity(actionContext: IControllerActionContext) {
            var quantity = this.getCurrentQuantity();
            var newValue: number = parseInt(actionContext.elementContext.val(), 10);

            if (isFinite(newValue)) {
                quantity.Value = Math.max(Math.min(newValue, quantity.Max), quantity.Min); // constraint newvalue to max and min.
            }

            this.renderAvailableQuantity(quantity).done();
        }

        public addLineItemToWishList(actionContext: IControllerActionContext) {

            this._membershipService.isAuthenticated().then(result => {
                if (result.IsAuthenticated) {
                    var vm = this.context.viewModel;
                    var busy = this.asyncBusy({ elementContext: actionContext.elementContext });
                    var analyticData = {
                        DisplayName: vm.DisplayName,
                        ListPrice: vm.ListPrice
                    };

                    this.eventHub.publish('wishListLineItemAdding', {
                        data: analyticData
                    });

                    this._wishListService.addLineItem(vm.productId, vm.selectedVariantId, 1, null, vm.RecurringOrderProgramName).then(data => {
                        var lineItem = data.Items.filter(it => it.ProductId === vm.productId && it.VariantId === vm.selectedVariantId)[0];
                        this.render('AddToWishList', { Loaded: true, IsInWishList: true, Id: lineItem.Id });
                    }).fin(() => busy.done());

                } else {
                    this.redirectToSignInBeforeAddToWishList();
                }
            });

        }

        public removeLineItemToWishList(actionContext: IControllerActionContext) {

            this._membershipService.isAuthenticated().then(result => {
                if (result.IsAuthenticated) {
                    var id = actionContext.elementContext.data('id');
                    var busy = this.asyncBusy({ elementContext: actionContext.elementContext });
                    this._wishListService.removeLineItem(id).then(data => {
                        this.render('AddToWishList', { Loaded: true, IsInWishList: false });
                    }).fin(() => busy.done());
                } else {
                    this.redirectToSignInBeforeAddToWishList();
                }
            });

        }

        protected redirectToSignInBeforeAddToWishList() {
            this._wishListService.getSignInUrl().then(signInUrl => {
                this._wishListService.clearCache();
                this.context.window.location.href = signInUrl + '?ReturnUrl=' + this.context.window.location.href;
            });
        }

        public addLineItem(actionContext: IControllerActionContext,
            recurringOrderFrequencyName?: string) {
            let busy = this.asyncBusy({ elementContext: actionContext.elementContext }),
                quantity = this.getCurrentQuantity(),
                vm = this.context.viewModel;
                
              this.addLineItemImpl(vm, vm.ListPrice, vm.selectedVariantId, quantity,
                recurringOrderFrequencyName)
                .then((data: any) => {
                    this.onAddLineItemSuccess(data);
                    actionContext.elementContext.focus();
                    return data;
                }, (reason: any) => {
                    this.onAddLineItemFailed(reason);
                    actionContext.elementContext.focus();
                    throw reason;
                })
                .then((data: any) => this.completeAddLineItem(quantity))
                .fin(() => busy.done());
        }

        protected onAddLineItemSuccess(data: any = undefined): void {
            ErrorHandler.instance().removeErrors();
        }

        protected onAddLineItemFailed(reason: any): void {
            console.error('Error on adding line item', reason);
            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        protected getCurrentQuantity() {
            var element: JQuery = $(this.context.container).find('[name="product-quantity"]');

            return {
                Min: parseInt(element.data('quantityMin'), 10),
                Max: parseInt(element.data('quantityMax'), 10),
                Value: parseInt(element.data('quantity'), 10)
            };
        }

        protected addLineItemImpl(product: any, price: string, variantId: string, quantity: any,
            recurringOrderFrequencyName?: string): Q.Promise<any> {
            return this.cartService.addLineItem(product, price, variantId, quantity.Value, this.getListNameForAnalytics(), recurringOrderFrequencyName);
        }

        protected completeAddLineItem(quantityAdded: any): Q.Promise<void> {

            return;
        }

        public selectImage(actionContext: IControllerActionContext) {

            actionContext.event.preventDefault();
            var target = actionContext.event.target;
            var mainSrc = $(target).attr('data-main-src');
            var zoomSrc = $(target).attr('data-zoom-src');

            if (target.tagName.toLowerCase() === 'img') {
                $(target).parents('[data-variant]').find('a').removeClass('active');
                $(target).parent('a').addClass('active');
                $('.product-main-img:visible').attr('src', mainSrc);

                var zoomThumbnail = $('.js-zoom-thumbnails').find('img[data-zoom-src="' + zoomSrc + '"]');
                zoomThumbnail.click();
            }

        }

        public zoomImage(actionContext: IControllerActionContext) {
            
            var target = actionContext.event.target;
            var zoomSrc = $(target).attr('data-zoom-src');

            if (target.tagName.toLowerCase() === 'img') {

                var zoomThumbnail = $('.js-zoom-thumbnails').find('img[data-zoom-src="' + zoomSrc + '"]');
                zoomThumbnail.click();
            }
        }

        public selectKva(actionContext: IControllerActionContext) {

            var selectionsToAdd = {};
            var propertyName: any = actionContext.elementContext.parents('[data-propertyname]').data('propertyname');
            var propertyDataType: any = actionContext.elementContext.parents('[data-propertydatatype]').data('propertydatatype');
            
            var formatter = new Orckestra.Composer.ProductFormatter();
            var value = formatter.convertToStronglyTyped(actionContext.elementContext.val(), propertyDataType);

            selectionsToAdd[propertyName] = value;

            this.productService.updateSelectedKvasWith(selectionsToAdd, this.concern);
        }

        protected calculatePrice(): Q.Promise<any> {

            return this.productService.calculatePrice(this.context.viewModel.productId, this.concern);
        }

        protected publishProductDataForAnalytics(vm: any, eventName: string): void {
            var data = ProductsHelper.getProductDataForAnalytics(vm, vm.selectedVariantId, vm.ListPrice, this.getListNameForAnalytics());

            this.eventHub.publish(eventName, { data });
        }

        protected getProductDataForAnalytics(vm: any): any {
            var productId: string = (vm.productId) ? vm.productId : vm.ProductId;

            var data = {
                List: this.getListNameForAnalytics(),
                ProductId: productId,
                DisplayName: vm.DisplayName,
                ListPrice: vm.ListPrice,
                Brand: vm.Brand,
                CategoryId: vm.CategoryId
            };

            return data;
        }

        protected getListNameForAnalytics(): string {
            throw new Error('ListName not defined for this controller');
        }

    }
}
