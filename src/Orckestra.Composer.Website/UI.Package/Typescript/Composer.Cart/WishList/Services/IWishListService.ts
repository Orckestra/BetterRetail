///<reference path='../../../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface IWishListService {
        addLineItem(productId: string, variantId?: string,
            quantity?: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Promise<any>;

        removeLineItem(lineItemId: string): Promise<any>;

        getWishListSummary(): Promise<any>;

        clearCache();
    }
}
