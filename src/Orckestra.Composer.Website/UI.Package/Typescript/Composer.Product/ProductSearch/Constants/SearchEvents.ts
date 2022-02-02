///<reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export enum SearchEvents {
        SortingChanged = 'sortingChanged',
        SingleFacetsChanged = 'singleFacetsChanged',
        MultiFacetChanged = 'multiFacetChanged',
        FacetsCleared = 'facetsCleared',
        FacetRemoved = 'facetRemoved',
        FacetsRemoved = 'facetsRemoved',
        SingleCategoryAdded = 'singleCategoryAdded',
        FacetsModalOpened = 'facetsModalOpened',
        FacetsModalClosed = 'facetsModalClosed',
        SearchRequested = 'searchRequested',
        SearchResultsLoaded = 'searchResultsLoaded',
        FacetsLoaded = 'facetsLoaded'
    }
}
