/// <reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/JqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerContext.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class SortBySearchController extends Orckestra.Composer.Controller {

        public sortingChanged(actionContext: IControllerActionContext) {
            var anchorContext = actionContext.elementContext,
                dataSortingType = anchorContext.data('sorting'),
                dataUrl = anchorContext.data('url'),
                resolvePageType = UrlHelper.resolvePageType();

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('sortingChanged', {
                data: {
                    sortingType: dataSortingType,
                    pageType: resolvePageType,
                    url: dataUrl
                }
            });
        }
    }
}
