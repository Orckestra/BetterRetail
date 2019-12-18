/// <reference path='../../../../../Composer.UI/Source/TypeScript/Events/IEventInformation.ts' />
/// <reference path='../../../../../Composer.UI/Source/TypeScript/Generics/Collections/IHashtable.ts' />
/// <reference path='../ISearchCriteriaOptions.ts' />

module Orckestra.Composer {
    export interface ISearchService {
        initialize(options: ISearchCriteriaOptions);

        singleFacetsChanged(eventInformation: IEventInformation);

        multiFacetChanged(eventInformation: IEventInformation);

        clearFacets(eventInformation: IEventInformation);

        removeFacet(eventInformation: IEventInformation);

        getSelectedFacets(): IHashTable<string|string[]>;
    }
}
