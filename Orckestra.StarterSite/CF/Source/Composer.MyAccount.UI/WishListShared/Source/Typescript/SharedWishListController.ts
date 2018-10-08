///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../../Composer.Cart.UI/WishList/Source/Typescript/WishListRepository.ts' />
///<reference path='../../../../Composer.Cart.UI/WishList/Source/Typescript/Services/WishListService.ts' />
///<reference path='../../../../Composer.Cart.UI/CartSummary/Source/Typescript/CartService.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Repositories/CartRepository.ts' />
///<reference path='../../../WishList/Source/Typescript/WishListController.ts' />

module Orckestra.Composer {

    export class SharedWishListController extends Orckestra.Composer.WishListController {


        public initialize() {

            super.initialize();
        }

        protected getListNameForAnalytics(): string {
            return 'Shared Wish List';
        }

    }
}
