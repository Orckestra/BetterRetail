///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./WishListController.ts' />

module Orckestra.Composer {

    export class MyWishListController extends Orckestra.Composer.WishListController {
        protected VueWishList: Vue;

        public initialize() {

            super.initialize();
        }

        protected getListNameForAnalytics(): string {
            return 'My Wish List';
        }
    }
}
