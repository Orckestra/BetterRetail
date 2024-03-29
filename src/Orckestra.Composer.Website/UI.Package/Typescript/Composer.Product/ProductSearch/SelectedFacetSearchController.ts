/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />
/// <reference path='../../Composer.ContentSearch/SearchParams.ts' />

module Orckestra.Composer {
    'use strict';

    export class SelectedFacetSearchController extends Orckestra.Composer.Controller {
        protected vueSelectedSearchFacets: Vue;

        public initialize() {
            super.initialize();
            const SelectedSortBy = this.context.container.data('selected-sort');
            const AvailableSortBys = this.context.container.data('available-sort');
            const itemsCount = this.context.container.data('items-count');
            const self = this;

            this.vueSelectedSearchFacets = new Vue({
                el: '#vueSelectedSearchFacets',
                components: {
                },
                data: {
                    ...this.context.viewModel,
                    SelectedSortBy,
                    TotalCount: itemsCount,
                    AvailableSortBys,
                    LandingPageUrls: this.context.container.data('landingpageurls') || [],
                    SelectedFacets: SearchService.getInstance() ? SearchService.getInstance().getSelectedFacets(): {}
                },
                mounted() {
                    self.eventHub.subscribe(SearchEvents.FacetsLoaded, this.onFacetsLoaded);
                    self.eventHub.subscribe(SearchEvents.SearchResultsLoaded, this.onFacetsLoaded);
                    self.eventHub.subscribe(SearchEvents.SearchRequested, this.onSearchRequested.bind(this));
                },
                computed: {
                },
                methods: {
                    sortingChanged(url) {
                        self.eventHub.publish(SearchEvents.SortingChanged, {data: {url}});
                    },
                    getFacetsCount() {
                        const getCount = (prev, next) => prev + (Array.isArray(this.SelectedFacets[next]) ? this.SelectedFacets[next].length : 1);
                        return Object.keys(this.SelectedFacets).reduce(getCount, 0);
                    },
                    onFacetsLoaded({data}) {
                        this.Facets = data.FacetSettings.SelectedFacets.Facets;
                        this.IsAllRemovable = data.FacetSettings.SelectedFacets.IsAllRemovable;
                        this.LandingPageUrls = data.LandingPageUrls || [];
                    },
                    onSearchRequested({data}): void {
                        this.SelectedFacets = data.selectedFacets;
                        const dataParams = new URLSearchParams(data.queryString);
                        this.SelectedSortBy = AvailableSortBys.find(function(sortBy, index){
                            const sortByParams = new URLSearchParams(sortBy.Url);
                            var sortDirection = sortByParams.get("sortDirection");
                            var dataDirection = dataParams.get("sortDirection");
                            var sortByVal = sortByParams.get("sortBy");
                            var dataSortBy = dataParams.get("sortBy");
                            return ((sortByVal === dataSortBy) && (sortDirection === dataDirection));
                        });
                    },
                    clearSelectedFacets(landingPageUrl) {
                        self.eventHub.publish(SearchEvents.FacetsCleared, { data: { landingPageUrl } });
                    },
                    removeSelectedFacet(facet, index) {
                        const categoryFacetFiledNamePrefix = 'CategoryLevel';
                        const categoryTreeRef = facet.FieldName.startsWith(categoryFacetFiledNamePrefix) && facet.FieldName;
                        const facetLandingPageUrl = categoryTreeRef && this.LandingPageUrls.length > index && this.LandingPageUrls[index];

                        if(facetLandingPageUrl || !categoryTreeRef) {
                            if(categoryTreeRef) {
                                // case, when remove category facet with landing page url and sub-categories selected
                                const getLevel = (f) => {
                                    const match = f.FieldName.match(/CategoryLevel(\d+)_Facet/);
                                    return match && match[1];
                                }
                                const level = getLevel(facet);
                                const data = this.Facets.filter(f => getLevel(f) > level).map(f => ({
                                    facetFieldName: f.FieldName,
                                    facetType: f.FacetType,
                                    facetValue: f.Value,
                                }))

                                self.eventHub.publish(SearchEvents.FacetsRemoved, { data });
                            }

                            self.eventHub.publish(SearchEvents.FacetRemoved, {
                                data: {
                                    facetFieldName: facet.FieldName,
                                    facetValue: facet.Value,
                                    facetType: facet.FacetType,
                                    facetLandingPageUrl: facetLandingPageUrl
                                }
                            });
                        } else if(categoryTreeRef) {
                            switch (facet.FacetType) {
                                case 'MultiSelect': {
                                    const currentFacets: any = [];
                                    SearchParams.getSearchParams().forEach(a => currentFacets.push(a));

                                    const data = {
                                        facetKey: facet.FieldName,
                                        facetValue: facet.Value,
                                        //    pageType,
                                        filter: this.Facets.reduce((filter, f) => {
                                            if (f.Value !== facet.Value && currentFacets.includes(f.FieldName)) {
                                                filter[f.FieldName] = f.FacetType === 'MultiSelect' ?
                                                    (filter[f.FieldName] || []).concat(f.Value) : f.Value;
                                            }
                                            return filter;
                                        }, {})
                                    };

                                    self.eventHub.publish(SearchEvents.MultiFacetChanged, {data});
                                    break;
                                }
                                case 'SingleSelect':
                                default:
                                    //remove also all child categories
                                    const parentCategoryElement = $('#categoriesTree').find('div[data-facetfieldname="' + categoryTreeRef + '"]');
                                    const checkedItems = parentCategoryElement.find('input:checked');
                                    const data = [];
                                    checkedItems.each(index => {
                                        let el = $(checkedItems[index]);
                                        data.push({
                                            facetFieldName: el.attr('name').replace('[]',''),
                                            facetValue: el.attr('value'),
                                            facetType: el.data('type')
                                        })
                                    });

                                    self.eventHub.publish(SearchEvents.FacetsRemoved, { data });
                            }
                        }
                    }
                }
            });
        }
    }
}
