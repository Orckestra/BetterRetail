///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/IControllerContext.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../../../Events/EventHub.ts' />
///<reference path='./IStoreInventoryService.ts' />


namespace Orckestra.Composer {
    'use strict';

    export class StoreInventoryService implements IStoreInventoryService {

        public getStoresInventory(param: GetStoresInventoryParam): Promise<any> {
            return ComposerClient.post('/api/storeinventory/storesinventory', param);
        }

        public getDefaultAddress(): Promise<any> {
            return ComposerClient.get('/api/customer/getdefaultaddress');
        }

        public getSkuSelection(productId: string): Promise<any> {
            var data = {
                ProductId: productId
            };
            return ComposerClient.post('/api/product/variantSelection', data);
        }
    }
}