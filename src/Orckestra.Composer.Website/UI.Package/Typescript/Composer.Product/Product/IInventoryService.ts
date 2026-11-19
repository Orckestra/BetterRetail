///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />

namespace Orckestra.Composer {

    export interface IInventoryService {

        isAvailableToSell(sku: string): Q.Promise<boolean>;
    }
}
