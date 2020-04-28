///<reference path='../Typings/tsd.d.ts' />
///<reference path='./Bootstrap.ts' />
///<reference path='./JQueryPlugins/IPopOverJqueryPlugin.ts' />
///<reference path='./IComposerConfiguration.ts' />
///<reference path='./Controller/SearchBoxController.ts' />
///<reference path='./Controller/AutocompleteSearchBoxController.ts' />
///<reference path='./Controller/PageNotFoundAnalyticsController.ts' />
///<reference path='./Controller/LanguageSwitchController.ts' />
///<reference path='./Controller/LazyController.ts' />
///<reference path='./Composer.Product/ProductSearch/SortBySearchController.ts' />
///<reference path='./Composer.Product/ProductSearch/FacetSearchController.ts' />
///<reference path='./Composer.Product/ProductSearch/SearchResultsController.ts' />
///<reference path='./Composer.Product/ProductSearch/SearchSummaryController.ts' />
///<reference path='./Composer.Product/ProductSearch/QuickViewController.ts' />
///<reference path='./Composer.Product/ProductSearch/SelectedFacetSearchController.ts' />
///<reference path='./Composer.Cart/AddToCartNotification/AddToCartNotificationController.ts' />
///<reference path='./Composer.Cart/CartSummary/FullCartController.ts' />
///<reference path='./Composer.Cart/MiniCart/MiniCartController.ts' />
///<reference path='./Composer.Cart/MiniCart/MiniCartSummaryController.ts' />
///<reference path='./Composer.Cart/Coupons/CouponController.ts' />
///<reference path='./Composer.Product/ProductDetail/ProductDetailController.ts' />
///<reference path='./Composer.Product/RelatedProducts/RelatedProductsController.ts' />
///<reference path='./Composer.Product/ProductDetail/ProductZoomController.ts' />
///<reference path='./Composer.Product/ProductDetail/RecurringOrderSignInFormController.ts' />
///<reference path='./Composer.Cart/OrderSummary/OrderSummaryController.ts' />
///<reference path='./Composer.MyAccount/AddressList/AddressListController.ts' />
///<reference path='./Composer.MyAccount/ChangePassword/ChangePasswordController.ts' />
///<reference path='./Composer.MyAccount/CreateAccount/CreateAccountController.ts' />
///<reference path='./Composer.MyAccount/EditAddress/EditAddressController.ts' />
///<reference path='./Composer.MyAccount/ForgotPassword/ForgotPasswordController.ts' />
///<reference path='./Composer.MyAccount/AccountHeader/AccountHeaderController.ts' />
///<reference path='./Composer.MyAccount/UpdateAccount/UpdateAccountController.ts' />
///<reference path='./Composer.MyAccount/NewPassword/NewPasswordController.ts' />
///<reference path='./Composer.MyAccount/ReturningCustomer/ReturningCustomerController.ts' />
///<reference path='./Composer.MyAccount/WishList/MyWishListController.ts' />
///<reference path='./Composer.MyAccount/WishListShared/SharedWishListController.ts' />
///<reference path='./Composer.MyAccount/WishList/WishListInHeaderController.ts' />
///<reference path='./Composer.MyAccount/SignInHeader/SignInHeaderController.ts' />
///<reference path='./Composer.MyAccount/MyAccount/MyAccountController.ts' />
///<reference path='./Composer.Cart/OrderHistory/CurrentOrdersController.ts' />
///<reference path='./Composer.Cart/OrderHistory/PastOrdersController.ts' />
///<reference path='./Composer.Cart/OrderDetails/OrderDetailsController.ts' />
///<reference path='./Composer.Cart/FindMyOrder/FindMyOrderController.ts' />
///<reference path='./ErrorHandling/ErrorController.ts' />

///<reference path='./Composer.Store/StoreLocator/StoreLocatorController.ts' />
///<reference path='./Composer.Store/StoreLocator/StoreDetailsController.ts' />
///<reference path='./Composer.Store/StoreDirectory/StoresDirectoryController.ts' />
///<reference path='./Composer.Store/StoreInventory/StoreInventoryController.ts' />
///<reference path='./Composer.MyAccount/RecurringSchedule/MyRecurringScheduleController.ts' />
///<reference path='./Composer.MyAccount/RecurringSchedule/MyRecurringScheduleDetailsController.ts' />
///<reference path='./Composer.MyAccount/RecurringCart/MyRecurringCartsController.ts' />
///<reference path='./Composer.MyAccount/RecurringCart/MyRecurringCartDetailsController.ts' />

///<reference path='./Composer.SingleCheckout/GuestCustomerInfoSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/ShippingSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/ShippingAddressSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/ShippingAddressRegisteredSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/ReviewCartSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/OrderSummarySingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/PaymentSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/BillingAddressSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/BillingAddressRegisteredSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/PickUpAddressSingleCheckoutController.ts' />
///<reference path='./Composer.SingleCheckout/OrderConfirmationController.ts' />

(() => {
    'use strict';

    // This file is currently used for the composer team so that we can deploy and hook into
    // our client-side code, but the starter site that ships, will ship with an App.ts
    // that will look like this.

    $(document).ready(() => {
        let composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
            plugins: [
                'AntiIFrameClickJacking',
                'ComposerValidationLocalization',
                'HelpBubbles',
                //'StickyAffix',
                'SlickCarousel',
                'FocusElement',
                'GoogleAnalytics'
            ],

            controllers: [
                { name: 'General.ErrorController', controller: Orckestra.Composer.ErrorController },
                { name: 'General.SearchBox', controller: Orckestra.Composer.SearchBoxController },
                { name: 'General.LanguageSwitch', controller: Orckestra.Composer.LanguageSwitchController },
                { name: 'General.AutocompleteSearchBox', controller: Orckestra.Composer.AutocompleteSearchBoxController },
                { name: 'General.Lazy', controller: Orckestra.Composer.LazyController },

                { name: 'Cart.FullCart', controller: Orckestra.Composer.FullCartController },
                { name: 'Cart.OrderSummary', controller: Orckestra.Composer.OrderSummaryController },
                { name: 'Cart.MiniCart', controller: Orckestra.Composer.MiniCartController },
                { name: 'Cart.MiniCartSummary', controller: Orckestra.Composer.MiniCartSummaryController },
                { name: 'Cart.Coupons', controller: Orckestra.Composer.CouponController },
                { name: 'Cart.AddToCartNotification', controller: Orckestra.Composer.AddToCartNotificationController },

                { name: 'Product.SortBySearch', controller: Orckestra.Composer.SortBySearchController },
                { name: 'Product.FacetSearch', controller: Orckestra.Composer.FacetSearchController },
                { name: 'Product.ProductDetail', controller: Orckestra.Composer.ProductDetailController },
                { name: 'Product.RelatedProducts', controller: Orckestra.Composer.RelatedProductController },
                { name: 'Product.SearchResults', controller: Orckestra.Composer.SearchResultsController },
                { name: 'Product.SearchSummary', controller: Orckestra.Composer.SearchSummaryController },
                { name: 'Product.QuickView', controller: Orckestra.Composer.QuickViewController },
                { name: 'Product.SelectedSearchFacets', controller: Orckestra.Composer.SelectedFacetSearchController },
                { name: 'Product.ProductZoom', controller: Orckestra.Composer.ProductZoomController },
                { name: 'Product.RecurringOrderSignInForm', controller: Orckestra.Composer.RecurringOrderSignInFormController },

                { name: 'SingleCheckout.GuestCustomerInfo', controller: Orckestra.Composer.GuestCustomerInfoSingleCheckoutController },
                { name: 'SingleCheckout.Shipping', controller: Orckestra.Composer.ShippingSingleCheckoutController },
                { name: 'SingleCheckout.ShippingAddress', controller: Orckestra.Composer.ShippingAddressSingleCheckoutController },
                {
                    name: 'SingleCheckout.ShippingAddressRegistered',
                    controller: Orckestra.Composer.ShippingAddressRegisteredSingleCheckoutController
                },
                { name: 'SingleCheckout.ReviewCart', controller: Orckestra.Composer.ReviewCartSingleCheckoutController },
                { name: 'SingleCheckout.OrderSummary', controller: Orckestra.Composer.OrderSummarySingleCheckoutController },
                { name: 'SingleCheckout.Payment', controller: Orckestra.Composer.PaymentSingleCheckoutController },
                { name: 'SingleCheckout.BillingAddress', controller: Orckestra.Composer.BillingAddressSingleCheckoutController },
                { name: 'SingleCheckout.BillingAddressRegistered', controller: Orckestra.Composer.BillingAddressRegisteredSingleCheckoutController },
                { name: 'SingleCheckout.PickUpStoreAddress', controller: Orckestra.Composer.PickUpAddressSingleCheckoutController },
                { name: 'SingleCheckout.OrderConfirmation', controller: Orckestra.Composer.OrderConfirmationController },

                { name: 'MyAccount.AddressList', controller: Orckestra.Composer.AddressListController },
                { name: 'MyAccount.ChangePassword', controller: Orckestra.Composer.ChangePasswordController },
                { name: 'MyAccount.CreateAccount', controller: Orckestra.Composer.CreateAccountController },
                { name: 'MyAccount.EditAddress', controller: Orckestra.Composer.EditAddressController },
                { name: 'MyAccount.ForgotPassword', controller: Orckestra.Composer.ForgotPasswordController },
                { name: 'MyAccount.AccountHeader', controller: Orckestra.Composer.AccountHeaderController },
                { name: 'MyAccount.UpdateAccount', controller: Orckestra.Composer.UpdateAccountController },
                { name: 'MyAccount.NewPassword', controller: Orckestra.Composer.NewPasswordController },
                { name: 'MyAccount.ReturningCustomer', controller: Orckestra.Composer.ReturningCustomerController },
                { name: 'MyAccount.SignInHeader', controller: Orckestra.Composer.SignInHeaderController },
                { name: 'MyAccount.MyWishList', controller: Orckestra.Composer.MyWishListController },
                { name: 'MyAccount.SharedWishList', controller: Orckestra.Composer.SharedWishListController },
                { name: 'MyAccount.WishListInHeader', controller: Orckestra.Composer.WishListInHeaderController },
                { name: 'MyAccount.MyRecurringSchedule', controller: Orckestra.Composer.MyRecurringScheduleController },
                { name: 'MyAccount.MyRecurringScheduleDetails', controller: Orckestra.Composer.MyRecurringScheduleDetailsController },
                { name: 'MyAccount.MyRecurringCarts', controller: Orckestra.Composer.MyRecurringCartsController },
                { name: 'MyAccount.MyRecurringCartDetails', controller: Orckestra.Composer.MyRecurringCartDetailsController },

                { name: 'Orders.CurrentOrders', controller: Orckestra.Composer.CurrentOrdersController },
                { name: 'Orders.PastOrders', controller: Orckestra.Composer.PastOrdersController },
                { name: 'Orders.OrderDetails', controller: Orckestra.Composer.OrderDetailsController },
                { name: 'Orders.FindMyOrder', controller: Orckestra.Composer.FindMyOrderController },

                { name: 'Store.Locator', controller: Orckestra.Composer.StoreLocatorController },
                { name: 'Store.Details', controller: Orckestra.Composer.StoreDetailsController },
                { name: 'Store.Directory', controller: Orckestra.Composer.StoresDirectoryController },
                { name: 'Store.Inventory', controller: Orckestra.Composer.StoreInventoryController },
                { name: 'PageNotFound.Analytics', controller: Orckestra.Composer.PageNotFoundAnalyticsController }
            ]
        };

        Orckestra.Composer.bootstrap(window, document, composerConfiguration);
    });
})();
