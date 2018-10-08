///<reference path='../../../../../Composer.UI/Source/Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IWishListService {
        addLineItem(productId: string, variantId?: string): Q.Promise<any>;

        removeLineItem(lineItemId: string): Q.Promise<any>;

        getWishListSummary(): Q.Promise<any>;

        clearCache();
    }
}
