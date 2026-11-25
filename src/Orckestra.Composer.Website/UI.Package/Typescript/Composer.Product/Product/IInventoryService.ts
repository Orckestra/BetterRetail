///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />

namespace Orckestra.Composer {

    export interface IInventoryService {

        isAvailableToSell(sku: string): Promise<boolean>;
    }
}
