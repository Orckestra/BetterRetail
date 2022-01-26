/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../Events/IEventHub.ts' />
/// <reference path='../../../Events/IEventInformation.ts' />
/// <reference path='../SearchCriteria.ts' />
/// <reference path='./ISearchService.ts' />
/// <reference path='../IFacet.ts' />
/// <reference path='../ISingleSelectCategory.ts' />
///<reference path='../../../Repositories/ISearchRepository.ts' />
///<reference path='../../../Repositories/SearchRepository.ts' />
/// <reference path='../Constants/SearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    const FacetsModalId = '#facetsModal';

    // TODO: Decouple window object from search service.
    export class SearchService implements ISearchService {
        protected _searchRepository: ISearchRepository = new SearchRepository();
        protected _searchCriteria: SearchCriteria;
        private _searchCriteriaBackup: any;
        private _baseSearchUrl: string = window.location.href.replace(window.location.search, '');
        public IsFacetsModalMode: Boolean = false;

        constructor(protected _eventHub: IEventHub, private _window: Window) {
             this._searchCriteria = new SearchCriteria(_eventHub, _window);
        }

        /**
         * Initializes the search service.
         *
         * param facetRegistry Facets available to the search service.
         */
        public initialize(options: ISearchCriteriaOptions) {
            this._searchCriteria.initialize(options);
            this.registerSubscriptions();
        }

        public singleFacetsChanged(eventInformation: IEventInformation) {
            var facetKey: string = eventInformation.data.facetKey,
                facetValue: string = eventInformation.data.facetValue;

            this._searchCriteria.addSingleFacet(facetKey, facetValue);
            this.search();
        }

        public sortingChanged(eventInformation: IEventInformation) {
            this._searchCriteria.clearAll();
            this._searchCriteria.loadFromQuerystring(eventInformation.data.url);
            this.search();
        }

        public getSelectedFacets(): IHashTable<string|string[]> {
            return this._searchCriteria.selectedFacets;
        }

        public multiFacetChanged(eventInformation: IEventInformation) {
            this._searchCriteria.updateMultiFacets(eventInformation.data.filter);
            this.search();
        }

        public clearFacets(eventInformation: IEventInformation) {
            var landingPageUrl: string = eventInformation.data.landingPageUrl;

            this._searchCriteria.clearFacets();

            if (landingPageUrl) {
                this._baseSearchUrl = landingPageUrl;
            }

            this.search();
        }

        public removeFacet(eventInformation: IEventInformation) {
            var facet: IFacet = <IFacet>eventInformation.data;

            this._searchCriteria.removeFacet(facet);

            if (facet.facetLandingPageUrl && facet.facetType === 'SingleSelect') {
                this._baseSearchUrl = facet.facetLandingPageUrl;

                //TODO: detect new categoryId
                this._window.location.href = this._baseSearchUrl + this._searchCriteria.toQuerystring();
            }

            this.search();
        }

        public removeFacets(eventInformation: IEventInformation) {
            const faces: [] = eventInformation.data;
            faces.forEach(f => this._searchCriteria.removeFacet(f as IFacet));

            this.search();
        }

        public addSingleSelectCategory(eventInformation: IEventInformation) {
            var singleSelectCategory: ISingleSelectCategory = <ISingleSelectCategory>eventInformation.data;

            this._baseSearchUrl = singleSelectCategory.categoryUrl;

            this.search();
        }

        public facetsModalOpened() {
            this.IsFacetsModalMode = true;
            this._searchCriteriaBackup = this._searchCriteria.toQuerystring();

            this.updateClearButtonState();
        }

        public facetsModalClosed() {
            this._searchCriteria.clearFacets();
            this._searchCriteria.loadFromQuerystring(this._searchCriteriaBackup);
            this.search();
            this.IsFacetsModalMode = false;
        }

        public facetsModalApply() {
            this.IsFacetsModalMode = false;
            this.search();
        }

        public facetsModalCancel() {
            this._searchCriteria.clearFacets();
            this.search();
        }

        private updateClearButtonState() {
            const clearAllButton = $(`${FacetsModalId} .modal--cancel`);
            const applyButton = $(`${FacetsModalId} .modal--confirm`);
            const selected = Object.keys(this.getSelectedFacets());

            if(selected.length === 0) {
                clearAllButton.attr('disabled', 'true')
            } else {
                clearAllButton.removeAttr('disabled')
            }

            applyButton.prop('disabled', this._searchCriteria.toQuerystring() === this._searchCriteriaBackup);
        }

        private registerSubscriptions() {
            this._eventHub.subscribe(SearchEvents.SortingChanged, this.sortingChanged.bind(this));
            this._eventHub.subscribe(SearchEvents.SingleFacetsChanged, this.singleFacetsChanged.bind(this));
            this._eventHub.subscribe(SearchEvents.MultiFacetChanged, this.multiFacetChanged.bind(this));
            this._eventHub.subscribe(SearchEvents.FacetsCleared, this.clearFacets.bind(this));
            this._eventHub.subscribe(SearchEvents.FacetRemoved, this.removeFacet.bind(this));
            this._eventHub.subscribe(SearchEvents.FacetsRemoved, this.removeFacets.bind(this));
            this._eventHub.subscribe(SearchEvents.SingleCategoryAdded, this.addSingleSelectCategory.bind(this));
            this._eventHub.subscribe(SearchEvents.FacetsModalOpened, this.facetsModalOpened.bind(this));
            this._eventHub.subscribe(SearchEvents.FacetsModalClosed, this.facetsModalClosed.bind(this));

            $(FacetsModalId).on('show.bs.modal', (event) => this.facetsModalOpened());
            $(FacetsModalId).on('hide.bs.modal', (event) => this.facetsModalClosed());
            $(FacetsModalId).on('click', '.modal--confirm',  this.facetsModalApply.bind(this));
            $(FacetsModalId).on('click', '.modal--cancel',  this.facetsModalCancel.bind(this));
        }

        protected search() {
            if (this.IsFacetsModalMode) {
                this.updateClearButtonState();

                if ($(FacetsModalId).hasClass('loading')) return;
                $(FacetsModalId).addClass('loading');

                const queryString = this._searchCriteria.toQuerystring();
                const { categoryId, queryName, queryType } = this._searchCriteria;

                var getFacetsPromise = categoryId ? this._searchRepository.getCategoryFacets(categoryId, queryString) :
                    (queryName ? this._searchRepository.getQueryFacets(queryName, queryType, queryString) :
                        this._searchRepository.getFacets(queryString));

                getFacetsPromise.then(result => this._eventHub.publish(SearchEvents.FacetsLoaded, { data: result }))
                    .fail(reason => console.log(reason))
                    .finally(() => $(FacetsModalId).removeClass('loading'));

            } else {
                const queryString = this._searchCriteria.toQuerystring();
                const { categoryId, queryName, queryType } = this._searchCriteria;

                this._eventHub.publish(SearchEvents.SearchRequested, { data: { categoryId, queryName, queryType, queryString } });

                this._window.history.pushState(this._window.history.state, "", this._baseSearchUrl + queryString);
              //  this._window.location.href = this._baseSearchUrl + this._searchCriteria.toQuerystring();
            }
        }
    }
}
