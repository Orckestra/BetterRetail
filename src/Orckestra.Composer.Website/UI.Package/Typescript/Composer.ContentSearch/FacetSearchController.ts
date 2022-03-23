/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/Controller.ts' />

///<reference path='../Repositories/ISearchRepository.ts' />
///<reference path='../Repositories/SearchRepository.ts' />
/// <reference path='./SearchParams.ts' />
/// <reference path='./Constants/ContentSearchEvents.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    const VisibleFacetsCount = 5;

    export class ContentFacetSearchController extends Orckestra.Composer.Controller {
        private VueFacets: Vue;
        protected searchRepository: ISearchRepository = new SearchRepository();


        public initialize() {
            super.initialize();
            this.initializeVueComponent();
        }

        private initializeVueComponent() {
            const { Facets, SelectedFacets } = this.context.viewModel;

            let self = this;
            this.VueFacets = new Vue({
                el: '#vueContentSearchFacets',
                components: {
                },
                data: {
                    Mode: {
                        Loading: false
                    },
                    SelectedFacets: null,
                    Facets: null
                },
                mounted() {
                    const selectedFacets = self.formatSelectedFacets(SelectedFacets)
                    this.SelectedFacets = selectedFacets;
                    this.Facets = self.formatFacets(Facets, selectedFacets)
                    self.eventHub.subscribe(ContentSearchEvents.SearchResultsLoaded, this.onSearchResultsLoaded);
                },
                methods: {
                    onFacetToggle(event: any, key: string): void {
                        const queryString = SearchParams.changeFacet(key, event.target.checked);
                        this.loadSearchResults({queryString});
                    },
                    removeSelectedFacet(key: string): void {
                        const queryString = SearchParams.changeFacet(key, false);
                        this.loadSearchResults({queryString});
                    },
                    loadSearchResults({queryString}): void {
                        SearchParams.pushState(queryString);
                        const currentTab = SearchParams.getLastSegment();

                        this.Mode.isLoading = true;
                        self.searchRepository.getContentSearchResults(queryString, currentTab).then(result => {
                            this.Mode.isLoading = false;
                            self.eventHub.publish(ContentSearchEvents.SearchResultsLoaded, { data: result });
                        });
                    },
                    onSearchResultsLoaded({data}): void {
                        const { Facets, SelectedFacets } = data;

                        const selectedFacets = self.formatSelectedFacets(SelectedFacets)
                        this.SelectedFacets = selectedFacets;
                        this.Facets = self.formatFacets(Facets, selectedFacets)
                    }
                }
            });
        }

        private formatSelectedFacets(selectedFacets): any[] {
            return (selectedFacets || []).reduce((accum, item) => accum.concat(item.Hits), [])
        }

        private formatFacets(facets, selectedFacets) {
            return facets.map((facet) => {
                const hits =  facet.Hits.map(hit => ({
                    ...hit,
                    isSelected: selectedFacets.some(x => x.Key === hit.Key)
                }))

                return {
                    ...facet,
                    visibleFacets: hits.slice(0, VisibleFacetsCount),
                    hiddenFacets: hits.slice(VisibleFacetsCount),
                };
            })
        }
    }
}
