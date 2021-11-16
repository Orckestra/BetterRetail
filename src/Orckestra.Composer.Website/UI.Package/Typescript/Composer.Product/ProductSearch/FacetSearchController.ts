/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
/// <reference path='./Services/SliderService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />
/// <reference path='./Facets/FacetTreeVueComponent.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    export class FacetSearchController extends Orckestra.Composer.Controller {
        private VueFacets: Vue;
        private _debounceHandle; // need to see how to fix the any for this.
        private _debounceTimeout: number = 500;
        private _searchService: ISearchService; // TODO: DI this, constructor injection via controller factory?
        private sliderService: SliderService; // TODO: DI this, constructor injection via controller factory?
        private sliderServicesInstances: IHashTable<SliderService> = {};


        public initialize() {
            super.initialize();
            this.initializeVueComponent();
         }

        private initializeVueComponent() {
            var { CategoryFacetValuesTree, Facets, PromotedFacetValues, SelectedFacets } = this.context.viewModel;
            let self = this;
            this.VueFacets = new Vue({
                el: '#vueSearchFacets',
                components: {
                    [FacetTreeVueComponent.componentName]: FacetTreeVueComponent.getComponent()
                },
                data: {
                    CategoryFacetValuesTree,
                    Facets,
                    PromotedFacetValues,
                    Mode: {
                        Loading: false
                    }
                },
                mounted() {
                    self.initializeServices();
                    self.eventHub.subscribe('facetsLoaded', ({data}) => {
                        this.CategoryFacetValuesTree = data.FacetSettings.CategoryFacetValuesTree;
                        this.Facets = data.ProductSearchResults.Facets;
                    });
                },
                methods: {
                    categoryFacetChanged(event, isSelected) {
                        self.categoryFacetChanged(event, isSelected);
                    },
                    IsValuesCollapsed(facet) {
                        return facet.OnDemandFacetValues.findIndex((n:any) => n.IsSelected) < 0;
                    }
                },
                updated() {
                    self.disposeRangeSlider();
                    self.initializeRangeSlider();
                }
            });

            new Vue({
                el: '#vueFacetsFilterBy',
                data: {
                    SelectedFacets
                },
                computed: {
                    FiltersCount() {
                        let factes = self._searchService.getSelectedFacets();
                        let facetKeys = Object.keys(factes);
                        const getCount = (prev, next) => prev + (Array.isArray(factes[next]) ? factes[next].length : 1);
                        return facetKeys.reduce(getCount, 0);
                    }
                }
            })
        }

        public multiFacetChanged(actionContext: IControllerActionContext) {
            if (!_.isEmpty(this._debounceHandle)) {
                this._debounceHandle.cancel();
            }

            var anchorContext = actionContext.elementContext,
                facetKey = anchorContext.attr('name'),
                facetValue = anchorContext.attr('value');

            this._debounceHandle = _.debounce(() => {
                this.publishMultiFacetChanged(facetKey, facetValue, UrlHelper.resolvePageType())
            }, 800);

            this._debounceHandle();
        }

        public dispose() {
            super.dispose();
            this.disposeRangeSlider();
        }

        private disposeRangeSlider() {
            Object.keys(this.sliderServicesInstances).forEach(sliderServiceKey => this.sliderServicesInstances[sliderServiceKey].dispose());
        }

        public categoryFacetChanged(event, isSelected) {
            var el = event.target,
                facetKey = el.dataset.facetfieldname,
                facetType = el.dataset.type,
                facetValue = el.dataset.facetvalue;

            if (facetType === 'SingleSelect') {
                if (isSelected) {
                    var checkedItems = $(el).parent().parent().find('[data-selected=true]');
                    var data = [];
                    checkedItems.each(index => {
                        let el = $(checkedItems[index]);
                        el[0]['checked'] = false;
                        data.push({
                            facetFieldName: el.data('facetfieldname'),
                            facetValue: el.data('facetvalue'),
                            facetType: el.data('type'),
                        })
                    });
                    this.eventHub.publish('facetsRemoved', { data });
                } else {
                    var parentDiv = $(el).parent().parent();
                    parentDiv.parent().find('input:checked').each((index, el: any) => {
                        if (el.dataset.selected) {
                            el['checked'] = false;
                        }
                    })
                    this.publishSingleFacetsChanged(facetKey, facetValue, UrlHelper.resolvePageType());
                }
            }

            if (facetType === 'MultiSelect') {
                if (!_.isEmpty(this._debounceHandle)) {
                    this._debounceHandle.cancel();
                }

                this._debounceHandle = _.debounce(() => {
                    this.publishMultiFacetChanged(facetKey, facetValue, UrlHelper.resolvePageType())
                }, 800);

                this._debounceHandle();
            }
        }

        public singleFacetChanged(actionContext: IControllerActionContext) {
            var anchorContext = actionContext.elementContext,
                facetKey = anchorContext.data('facetfieldname'),
                facetValue = anchorContext.data('facetvalue'),
                facetType = anchorContext.data('type'),
                isSelected = anchorContext.data('selected');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();


            if (isSelected) {
                anchorContext.removeClass('selected');
                var data = {
                    facetFieldName: facetKey,
                    facetValue,
                    facetType
                }
                this.eventHub.publish('facetRemoved', { data });
            } else {
                var parentDiv = anchorContext.parent().parent();
                parentDiv.find('a').removeClass('selected');
                anchorContext.addClass('selected');
                this.publishSingleFacetsChanged(facetKey, facetValue, UrlHelper.resolvePageType());
            }
        }

        protected publishSingleFacetsChanged(facetKey, facetValue, pageType) {
            this.eventHub.publish('singleFacetsChanged', {
                data: {
                    facetKey,
                    facetValue,
                    pageType
                }
            });
        }

        protected publishMultiFacetChanged(facetKey, facetValue, pageType) {
            this.eventHub.publish('multiFacetChanged', {
                data: {
                    facetKey,
                    facetValue,
                    pageType,
                    filter: (<ISerializeObjectJqueryPlugin>$('form[name="searchFacets"]', this.context.container)).serializeObject()
                }
            });
        }

        public refineByRange(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();
            var container = actionContext.elementContext.closest('[data-facetfieldname]');
            var sliderServiceInstance = this.sliderServicesInstances[container.data('facetfieldname')];

            var values = sliderServiceInstance.getValues();
            var key = sliderServiceInstance.getKey();

            this.publishSingleFacetsChanged(key, values.join('|'), UrlHelper.resolvePageType());
        }

        private initializeServices() {
            var correctedSearchTerm: string = this.context.container.attr('data-corrected-search-term');
            var categoryId: string = this.context.container.attr('data-categoryId');

            this._searchService = new SearchService(this.eventHub, window);
            this._searchService.initialize({
                facetRegistry: this.buildFacetRegistry(),
                correctedSearchTerm: correctedSearchTerm,
                categoryId
            });

            this.initializeRangeSlider();
        }

        private initializeRangeSlider() {
            const selectedFacets: IHashTable<string|string[]> = this._searchService.getSelectedFacets();
            this.context.container.find('[data-facettype="Range"]').each((index, element) => {
                const facetFieldName = $(element).data('facetfieldname');
                const serviceInstance = new SliderService($(element), this.eventHub);

                serviceInstance.initialize(selectedFacets[facetFieldName]);
                this.sliderServicesInstances[facetFieldName] = serviceInstance;
            });
        }

        private buildFacetRegistry(): IHashTable<string> {
            var facetRegistry: IHashTable<string> = {};

            $('[data-facettype]', this.context.container)
                .add($('#selectedFacets [data-facetfieldname]', this.context.container))
                .each((index: number, item: HTMLElement) => {
                    var facetType: string,
                        facetFieldName: string,
                        facetGroup: JQuery = $(item);

                    facetFieldName = facetGroup.data('facetfieldname');
                    facetType = (<string>facetGroup.data('facettype')).toLowerCase();
                    facetRegistry[facetFieldName] = facetType;
                });

            return facetRegistry;
        }
    }
}
