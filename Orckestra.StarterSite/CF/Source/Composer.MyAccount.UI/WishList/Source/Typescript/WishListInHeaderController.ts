///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Events/EventScheduler.ts' />
///<reference path='../../../../Composer.Cart.UI/WishList/Source/TypeScript/WishListRepository.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Events/EventHub.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Events/IEventInformation.ts' />
///<reference path='../../../../Composer.MyAccount.UI/Common/Source/Typescript/MyAccountEvents.ts' />
///<reference path='../../../../Composer.Cart.UI/WishList/Source/Typescript/Services/WishListService.ts' />

module Orckestra.Composer {
    'use strict';

    export class WishListInHeaderController extends Orckestra.Composer.Controller {

        private _wishListService: IWishListService = new WishListService(new WishListRepository(), this.eventHub);

        public initialize() {

            super.initialize();

            this.initializeWishListQuantity();
            this.registerSubscriptions();
        }

        private initializeWishListQuantity(): void {

            if (!this.context.viewModel.IsAuthenticated) {
                this._wishListService.clearCache();
            }

            this._wishListService.getWishListSummary()
                .done(wishList => {

                    if (!_.isEmpty(wishList)) {
                        this.renderWishList(wishList);
                    }
                });
        }

        protected registerSubscriptions(): void {

            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            this.eventHub.subscribe('wishListUpdated', (e: IEventInformation) => this.onWishListUpdated(e));

            loggedOutScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
            loggedInScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
        }

        protected onWishListUpdated(e: IEventInformation): void {

            var wishList = e.data;
            this.renderWishList(wishList);
        }

        protected onRefreshUser(e: IEventInformation): Q.Promise<any> {

            return this._wishListService.clearCache();
        }

        protected renderWishList(wishList: any): void {

            this.render('WishListQuantity', wishList);
        }

        protected onError(reason: any): void {
            console.error(`An error occured while rendering the wishList with the WishListInHeader.`, reason);
        }
    }
}
