///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='./GetStoresInventoryParam.ts' />

namespace Orckestra.Composer {
    export interface IStoreInventoryService {
        getStoresInventory(param: GetStoresInventoryParam): Promise<any>;

        getDefaultAddress(): Promise<any>;

        getSkuSelection(productId: string): Promise<any>;
    }
}