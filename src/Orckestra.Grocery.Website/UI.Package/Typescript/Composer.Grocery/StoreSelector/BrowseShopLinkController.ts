///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../SelectedStoreService.ts' />

module Orckestra.Composer {
    export class BrowseShopLinkController extends Controller {
        protected selectedStoreService: ISelectedStoreService = SelectedStoreService.instance();

        public initialize() {
            super.initialize();
        }

        public browseShop(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();
            let browseShopUrl = actionContext.elementContext.attr('href');
            this.selectedStoreService.disableForcingToSelectStore().then(() => window.location.href = browseShopUrl);
        }
    }
}
