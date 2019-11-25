/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/JqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
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

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetRemoved', {
                data: {
                    facetFieldName: removeFacetButton.data('facetfieldname'),
                    facetValue: removeFacetButton.data('facetvalue'),
                    facetType: removeFacetButton.data('facettype'),
                    facetLandingPageUrl: removeFacetButton.data('facetlandingpageurl')
                }
            });
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
