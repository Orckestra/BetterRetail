///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />

module Orckestra.Composer {

    export interface IInventoryService {

        isAvailableToSell(sku: string): Q.Promise<boolean>;

        clearCache(): void;

        getProductsAvailability(Skus: string[]): Q.Promise<any>;
    }
}
