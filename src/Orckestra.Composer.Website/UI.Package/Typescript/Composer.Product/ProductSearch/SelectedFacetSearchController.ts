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
            var facetLandingPageUrl = removeFacetButton.data('facetlandingpageurl');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            if(facetLandingPageUrl || !categoryTreeRef) {
                this.eventHub.publish('facetRemoved', {
                    data: {
                        facetFieldName: removeFacetButton.data('facetfieldname'),
                        facetValue: removeFacetButton.data('facetvalue'),
                        facetType: removeFacetButton.data('facettype'),
                        facetLandingPageUrl: facetLandingPageUrl
                    }
                });
            } else {
                if (categoryTreeRef) {
                    //remove also all child categories
                    var parentCategoryElement = $('#categoriesTree').find('div[data-facetfieldname="' + categoryTreeRef + '"]');
                    var checkedItems = parentCategoryElement.find('input:checked');
                    var data = [];
                    checkedItems.each(index => {
                        let el = $(checkedItems[index]);
                        data.push({
                            facetFieldName: el.attr('name').replace('[]',''),
                            facetValue: el.attr('value'),
                            facetType: el.data('type')
                        })
                    });
                    this.eventHub.publish('facetsRemoved', { data });
                }
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
    }
}
