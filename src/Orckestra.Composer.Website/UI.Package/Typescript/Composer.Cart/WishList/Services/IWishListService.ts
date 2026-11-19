///<reference path='../../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface IWishListService {
        addLineItem(productId: string, variantId?: string,
            quantity?: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any>;

        removeLineItem(lineItemId: string): Q.Promise<any>;

        getWishListSummary(): Q.Promise<any>;

        clearCache();
    }
}
