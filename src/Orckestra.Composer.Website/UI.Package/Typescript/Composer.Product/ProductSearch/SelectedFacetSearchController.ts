/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    export class SelectedFacetSearchController extends Orckestra.Composer.Controller {

        public initialize() {
            super.initialize();
        }

        public removeSelectedFacet(actionContext: IControllerActionContext) {
            var removeFacetButton = actionContext.elementContext;
            var categoryTreeRef = removeFacetButton.data('categorytree');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            if (categoryTreeRef) {
                //remove also all child categories
                var parentCategoryElement = $('#categoriesTree').find('[data-facetfieldname="' + categoryTreeRef + '"]');
                var checkedInputs = parentCategoryElement.find('input:checked');
                var data = [];
                checkedInputs.each(index => {
                    data.push({
                        facetFieldName: $(checkedInputs[index]).attr('name').replace('[]',''),
                        facetValue: $(checkedInputs[index]).attr('value'),
                        facetType: $(checkedInputs[index]).attr('type'),
                    })
                });
                this.eventHub.publish('facetsRemoved', { data });
            } else {
                this.eventHub.publish('facetRemoved', {
                    data: {
                        facetFieldName: removeFacetButton.data('facetfieldname'),
                        facetValue: removeFacetButton.data('facetvalue'),
                        facetType: removeFacetButton.data('facettype'),
                        facetLandingPageUrl: removeFacetButton.data('facetlandingpageurl')
                    }
                });
            }          
        }

        public clearSelectedFacets(actionContext: IControllerActionContext) {
            var clearFacetsButton = actionContext.elementContext;

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetsCleared', {
               data: { landingPageUrl: clearFacetsButton.data('landingpageurl') }
            });
        }

        public addSingleSelectCategory(actionContext: IControllerActionContext) {
            var singleSelectCategory = actionContext.elementContext,
                anchorContext = actionContext.elementContext,
                facetFieldName = anchorContext.data('facetfieldname'),
                facetValue = anchorContext.data('facetvalue');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('singleCategoryAdded', {
                data: {
                    categoryUrl: singleSelectCategory.data('categoryurl'),
                    facetKey: facetFieldName,
                    facetValue: facetValue,
                    pageType: UrlHelper.resolvePageType()
                }
            });
        }
    }
}
