///<reference path='../Typings/tsd.d.ts' />
///<reference path='./Bootstrap.ts' />
///<reference path='./JQueryPlugins/IPopOverJqueryPlugin.ts' />
///<reference path='./IComposerConfiguration.ts' />
///<reference path='./Controller/SearchBoxController.ts' />
///<reference path='./Controller/PageNotFoundAnalyticsController.ts' />
///<reference path='./Controller/LanguageSwitchController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/SortBySearchController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/FacetSearchController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/SearchResultsController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/SearchSummaryController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/QuickViewController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSearch/Source/TypeScript/SelectedFacetSearchController.ts' />
///<reference path='../../../Composer.Cart.UI/AddToCartNotification/Source/TypeScript/AddToCartNotificationController.ts' />
///<reference path='../../../Composer.Cart.UI/CartSummary/Source/TypeScript/FullCartController.ts' />
///<reference path='../../../Composer.Cart.UI/MiniCart/Source/TypeScript/MiniCartController.ts' />
///<reference path='../../../Composer.Cart.UI/MiniCart/Source/TypeScript/MiniCartSummaryController.ts' />
///<reference path='../../../Composer.Cart.UI/Coupons/Source/Typescript/CouponController.ts' />
///<reference path='../../../Composer.Product.UI/ProductDetail/Source/TypeScript/ProductDetailController.ts' />
///<reference path='../../../Composer.Product.UI/RelatedProducts/Source/TypeScript/RelatedProductsController.ts' />
///<reference path='../../../Composer.Product.UI/ProductSpecification/Source/TypeScript/ProductSpecificationsController.ts' />
///<reference path='../../../Composer.Cart.UI/OrderSummary/Source/TypeScript/OrderSummaryController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutGuestCustomerInfo/Source/TypeScript/GuestCustomerInfoCheckoutController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutShippingAddress/Source/TypeScript/ShippingAddressCheckoutController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutShippingMethod/Source/TypeScript/ShippingMethodCheckoutController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutBillingAddress/Source/TypeScript/BillingAddressCheckoutController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutBillingAddressRegistered/Source/TypeScript/BillingAddressRegisteredCheckoutController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutOrderConfirmation/Source/TypeScript/CheckoutOrderConfirmationController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutComplete/Source/TypeScript/CheckoutCompleteController.ts' />
///<reference path='../../../Composer.MyAccount.UI/AddressList/Source/TypeScript/AddressListController.ts' />
///<reference path='../../../Composer.MyAccount.UI/ChangePassword/Source/TypeScript/ChangePasswordController.ts' />
///<reference path='../../../Composer.MyAccount.UI/CreateAccount/Source/TypeScript/CreateAccountController.ts' />
///<reference path='../../../Composer.MyAccount.UI/EditAddress/Source/TypeScript/EditAddressController.ts' />
///<reference path='../../../Composer.MyAccount.UI/ForgotPassword/Source/TypeScript/ForgotPasswordController.ts' />
///<reference path='../../../Composer.MyAccount.UI/AccountHeader/Source/TypeScript/AccountHeaderController.ts' />
///<reference path='../../../Composer.MyAccount.UI/UpdateAccount/Source/TypeScript/UpdateAccountController.ts' />
///<reference path='../../../Composer.MyAccount.UI/NewPassword/Source/TypeScript/NewPasswordController.ts' />
///<reference path='../../../Composer.MyAccount.UI/ReturningCustomer/Source/TypeScript/ReturningCustomerController.ts' />
///<reference path='../../../Composer.MyAccount.UI/WishList/Source/TypeScript/MyWishListController.ts' />
///<reference path='../../../Composer.MyAccount.UI/WishListShared/Source/TypeScript/SharedWishListController.ts' />
///<reference path='../../../Composer.MyAccount.UI/WishList/Source/TypeScript/WishListInHeaderController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutShippingAddressRegistered/Source/TypeScript/ShippingAddressRegisteredController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutOrderSummary/Source/TypeScript/CheckoutOrderSummaryController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutOrderSummary/Source/TypeScript/CompleteCheckoutOrderSummaryController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutPayment/Source/TypeScript/CheckoutPaymentController.ts' />
///<reference path='../../../Composer.Cart.UI/CheckoutCommon/Source/TypeScript/CheckoutNavigationController.ts' />
///<reference path='../../../Composer.MyAccount.UI/SignInHeader/Source/TypeScript/SignInHeaderController.ts' />
///<reference path='../../../Composer.MyAccount.UI/MyAccount/Source/TypeScript/MyAccountController.ts' />
///<reference path='../../../Composer.Cart.UI/OrderHistory/Source/TypeScript/CurrentOrdersController.ts' />
///<reference path='../../../Composer.Cart.UI/OrderHistory/Source/TypeScript/PastOrdersController.ts' />
///<reference path='../../../Composer.Cart.UI/OrderDetails/Source/TypeScript/OrderDetailsController.ts' />
///<reference path='../../../Composer.Cart.UI/FindMyOrder/Source/TypeScript/FindMyOrderController.ts' />
///<reference path='./ErrorHandling/ErrorController.ts' />

///<reference path='../../../Composer.Store.UI/StoreLocator/Source/TypeScript/StoreLocatorController.ts' />
///<reference path='../../../Composer.Store.UI/StoreLocator/Source/TypeScript/StoreDetailsController.ts' />
///<reference path='../../../Composer.Store.UI/StoreDirectory/Source/TypeScript/StoresDirectoryController.ts' />
///<reference path='../../../Composer.Store.UI/StoreInventory/Source/TypeScript/StoreInventoryController.ts' />

(() => {
    'use strict';

    // This file is currently used for the composer team so that we can deploy and hook into
    // our client-side code, but the starter site that ships, will ship with an App.ts
    // that will look like this.

    $(document).ready(() => {
        var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
            plugins: [
                'AntiIFrameClickJacking',
                'ComposerValidationLocalization',
                'HelpBubbles',
                'StickyAffix',
                'SlickCarousel',
                'FocusElement',
                'GoogleAnalytics',
                'AccessibleMegaMenu',
                'ParsleyAccessibility'
            ],

            controllers: [
                { name: 'General.ErrorController', controller: Orckestra.Composer.ErrorController },
                { name: 'General.SearchBox', controller: Orckestra.Composer.SearchBoxController },
                { name: 'General.LanguageSwitch', controller: Orckestra.Composer.LanguageSwitchController },

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
                { name: 'Product.ProductSpecifications', controller: Orckestra.Composer.ProductSpecificationsController },
                { name: 'Product.ProductZoom', controller: Orckestra.Composer.ProductZoomController },

                { name: 'Checkout.GuestCustomerInfo', controller: Orckestra.Composer.GuestCustomerInfoCheckoutController },
                { name: 'Checkout.ShippingAddress', controller: Orckestra.Composer.ShippingAddressCheckoutController },
                { name: 'Checkout.ShippingAddressRegistered', controller: Orckestra.Composer.ShippingAddressRegisteredController },
                { name: 'Checkout.ShippingMethod', controller: Orckestra.Composer.ShippingMethodCheckoutController },
                { name: 'Checkout.OrderSummary', controller: Orckestra.Composer.CheckoutOrderSummaryController },
                { name: 'Checkout.CompleteOrderSummary', controller: Orckestra.Composer.CompleteCheckoutOrderSummaryController },
                { name: 'Checkout.CheckoutComplete', controller: Orckestra.Composer.CheckoutCompleteController },
                { name: 'Checkout.CheckoutOrderConfirmation', controller: Orckestra.Composer.CheckoutOrderConfirmationController },
                { name: 'Checkout.BillingAddress', controller: Orckestra.Composer.BillingAddressCheckoutController },
                { name: 'Checkout.BillingAddressRegistered', controller: Orckestra.Composer.BillingAddressRegisteredCheckoutController },
                { name: 'Checkout.Payment', controller: Orckestra.Composer.CheckoutPaymentController },
                { name: 'Checkout.Navigation', controller: Orckestra.Composer.CheckoutNavigationController },

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
                { name: 'MyAccount.MyAccountMenu', controller: Orckestra.Composer.MyAccountController },
                { name: 'MyAccount.MyWishList', controller: Orckestra.Composer.MyWishListController },
                { name: 'MyAccount.SharedWishList', controller: Orckestra.Composer.SharedWishListController },
                { name: 'MyAccount.WishListInHeader', controller: Orckestra.Composer.WishListInHeaderController },

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
