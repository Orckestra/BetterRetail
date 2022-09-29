///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export enum ProductEvents {
        LineItemAdding = 'lineItemAdding',
        LineItemRemoving = 'lineItemRemoving',
        LineItemAdded = 'lineItemAddedToCart',
        LineItemUpdated = 'lineItemUpdated',
        WishListUpdating = 'wishListUpdating',
        WishListUpdated = 'wishListUpdated',
        ProductClick = 'productClick'
    }
}
