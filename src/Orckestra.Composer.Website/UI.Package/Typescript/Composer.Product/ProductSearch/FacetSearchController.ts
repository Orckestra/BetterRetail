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
            var { CategoryFacetValuesTree, Facets, PromotedFacetValues } = this.context.viewModel;
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
                    categoryFacetClicked(event, isSelected) {
                        self.categoryFacetChanged(event, isSelected);
                    }
                }

            });
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
            }, 250);

            this._debounceHandle();
        }

        public dispose() {
            super.dispose();
            Object.keys(this.sliderServicesInstances).forEach(sliderServiceKey => this.sliderServicesInstances[sliderServiceKey].dispose());
        }

        public singleFacetChanged(actionContext: IControllerActionContext) {
            var anchorContext = actionContext.elementContext,
                facetKey = anchorContext.data('facetfieldname'),
                facetValue = anchorContext.data('facetvalue');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.publishSingleFacetsChanged(facetKey, facetValue, UrlHelper.resolvePageType());
        }

        public categoryFacetChanged(event, isSelected) {

            var element = $(event.target),
                facetKey = element.attr('name'),
                facetValue = element.attr('value'),
                type = element.data('type'),
                categoryurl = element.data('categoryurl'),
                parentcategoryurl = element.data('parentcategoryurl'),
                pageType = UrlHelper.resolvePageType(),
                checked = isSelected,
                checkedCategories = element.parent().parent().find('input:checked');

            if (checked) {
                //unselect all sub-categories
                checkedCategories.each(index => {
                    var elem: any = checkedCategories[index];
                    elem.checked = false;
                })
            }

            if (categoryurl) {
                //if browse category - redirect to category page
                this.eventHub.publish('singleCategoryAdded', {
                    data: {
                        categoryUrl: checked ? parentcategoryurl : categoryurl,
                        facetKey: 'category',
                        facetValue: facetValue,
                        pageType
                    }
                });

                return;
            }


            if (type === 'SingleSelect') {
                if (checked) {
                    var data = [];
                    checkedCategories.each(index => {
                        data.push({
                            facetFieldName: $(checkedCategories[index]).attr('name').replace('[]', ''),
                            facetValue: $(checkedCategories[index]).attr('value'),
                            facetType: $(checkedCategories[index]).attr('type'),
                        })
                    });
                    this.eventHub.publish('facetsRemoved', { data });
                } else {
                    this.publishSingleFacetsChanged(facetKey, facetValue, pageType);
                }
            }

            if (type === 'MultiSelect') {
                if (!_.isEmpty(this._debounceHandle)) {
                    this._debounceHandle.cancel();
                }
                this._debounceHandle = _.debounce(() => {
                    this.publishMultiFacetChanged(facetKey, facetValue, pageType);
                }, 350);

                this._debounceHandle();
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
            var selectedFacets: IHashTable<string|string[]>;
            var correctedSearchTerm: string = this.context.container.attr('data-corrected-search-term');

            this._searchService = new SearchService(this.eventHub, window);
            this._searchService.initialize({
                facetRegistry: this.buildFacetRegistry(),
                correctedSearchTerm: correctedSearchTerm
            });
            selectedFacets = this._searchService.getSelectedFacets();

            this.context.container.find('[data-facettype="Range"]').each((index, element) => {
                var facetFieldName = $(element).data('facetfieldname');
                var serviceInstance = new SliderService($(element), this.eventHub);

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
