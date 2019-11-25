///<reference path='../../../RelatedProducts/Source/TypeScript/ProductIdentifierDto.ts' />

module Orckestra.Composer {

    export interface IInventoryService {

        isAvailableToSell(sku: string): Q.Promise<boolean>;
    }
}
