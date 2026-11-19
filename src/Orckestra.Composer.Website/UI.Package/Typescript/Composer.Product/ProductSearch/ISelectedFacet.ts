///<reference path='./IFacet.ts' />

namespace Orckestra.Composer {
    export interface ISelectedFacet {
        facetFieldName: string;
        selectedValues: string[];
        isFacetArray: boolean;
    }
}
