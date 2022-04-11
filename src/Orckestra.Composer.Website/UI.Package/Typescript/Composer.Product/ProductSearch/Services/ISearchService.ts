/// <reference path='../../../Events/IEventInformation.ts' />
/// <reference path='../../../Generics/Collections/IHashTable.ts' />
/// <reference path='../ISearchCriteriaOptions.ts' />

module Orckestra.Composer {
    export interface ISearchService {
        initialize(options: ISearchCriteriaOptions);

        updateFacetRegistry(facetRegistry: IHashTable<string>): void;

        singleFacetsChanged(eventInformation: IEventInformation);

        multiFacetChanged(eventInformation: IEventInformation);

        clearFacets(eventInformation: IEventInformation);

        removeFacet(eventInformation: IEventInformation);

        getSelectedFacets(): IHashTable<string|string[]>;
    }
}
