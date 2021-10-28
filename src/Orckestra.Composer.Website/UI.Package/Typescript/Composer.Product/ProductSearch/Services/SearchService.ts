/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../Events/IEventHub.ts' />
/// <reference path='../../../Events/IEventInformation.ts' />
/// <reference path='../SearchCriteria.ts' />
/// <reference path='./ISearchService.ts' />
/// <reference path='../IFacet.ts' />
/// <reference path='../ISingleSelectCategory.ts' />
///<reference path='../../../Repositories/ISearchRepository.ts' />
///<reference path='../../../Repositories/SearchRepository.ts' />

module Orckestra.Composer {
    'use strict';

    const FacetsModalId = '#facetsModal';

    // TODO: Decouple window object from search service.
    export class SearchService implements ISearchService {
        protected _searchRepository: ISearchRepository;
        protected _searchCriteria: SearchCriteria;
        private _searchCriteriaBackup: any;
        private _baseSearchUrl: string = window.location.href.replace(window.location.search, '');
        private _baseUrl: string = this._baseSearchUrl.replace(window.location.pathname, '');
        private _facetRegistry: IHashTable<string> = {};
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
            this.registerSubscriptions();
            this._searchCriteria.initialize(options);
            this._searchRepository = new SearchRepository();
            $(FacetsModalId).on('show.bs.modal', (event) => this.facetsModalOpened());
            $(FacetsModalId).on('hide.bs.modal', (event) => this.facetsModalClosed());
            $(FacetsModalId).on('click', '.modal--confirm',  this.facetsModalApply.bind(this));
            $(FacetsModalId).on('click', '.modal--cancel',  this.facetsModalCancel.bind(this));
        }

        public singleFacetsChanged(eventInformation: IEventInformation) {
            var facetKey: string = eventInformation.data.facetKey,
                facetValue: string = eventInformation.data.facetValue;

            this._searchCriteria.addSingleFacet(facetKey, facetValue);
            this.search();
        }

        public sortingChanged(eventInformation: IEventInformation) {
            var dataUrl = eventInformation.data.url;
            this._window.location.href = dataUrl;
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
            }

            this.search();
        }

        public removeFacets(eventInformation: IEventInformation) {
            var faces: [] = eventInformation.data;

            faces.forEach(f => {
                var facet: IFacet = <IFacet>f;
                this._searchCriteria.removeFacet(facet);
            })

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
        }

        public facetsModalClosed() {
            this.IsFacetsModalMode = false;
        }

        public facetsModalApply() {
            $(FacetsModalId).modal('hide');
            this.search();
        }

        public facetsModalCancel() {
            this._searchCriteria.loadFromQuerystring(this._searchCriteriaBackup)
        }

        private registerSubscriptions() {
            this._eventHub.subscribe('sortingChanged', this.sortingChanged.bind(this));
            this._eventHub.subscribe('singleFacetsChanged', this.singleFacetsChanged.bind(this));
            this._eventHub.subscribe('multiFacetChanged', this.multiFacetChanged.bind(this));
            this._eventHub.subscribe('facetsCleared', this.clearFacets.bind(this));
            this._eventHub.subscribe('facetRemoved', this.removeFacet.bind(this));
            this._eventHub.subscribe('facetsRemoved', this.removeFacets.bind(this));
            this._eventHub.subscribe('singleCategoryAdded', this.addSingleSelectCategory.bind(this));
            this._eventHub.subscribe('facetsModalOpened', this.facetsModalOpened.bind(this));
            this._eventHub.subscribe('facetsModalClosed', this.facetsModalClosed.bind(this));
        }

        protected search() {
            if (this.IsFacetsModalMode) {
                if ($(FacetsModalId).hasClass('loading')) return;
                $(FacetsModalId).addClass('loading');
                this._searchRepository.getFacets(this._searchCriteria.toQuerystring()).then(result => {
                    this._eventHub.publish('facetsLoaded', { data: result })
                })
                    .fail(reason => console.log(reason))
                    .finally(() => $(FacetsModalId).removeClass('loading'));
            } else {
                this._window.location.href = this._baseSearchUrl + this._searchCriteria.toQuerystring();
            }
        }
    }
}
