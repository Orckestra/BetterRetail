/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />
///<reference path='./Services/MultiKeywordsSearchService.ts' />
///<reference path='./Services/IMultiKeywordsSearchService.ts' />
/// <reference path='../../Composer.ContentSearch/SearchParams.ts' />

module Orckestra.Composer {
    'use strict';

    export class MultiKeywordsSearchController extends Orckestra.Composer.Controller {
        protected vueMultiKeywordsSearch: Vue;
        protected multiKeywordSearchService: IMultiKeywordsSearchService = MultiKeywordsSearchService.instance();

        public initialize() {
            super.initialize();
            const self = this;

            this.vueMultiKeywordsSearch = new Vue({
                el: '#vueMultiKeywordsSearch',
                data: {
                    Keywords: [],
                    SelectedKeyword: '*'
                },
                mounted() {
                    self.multiKeywordSearchService.getKeywords().then(items => {
                        this.Keywords = items;
                        let keyword = SearchParams.getKeyword();
                        this.SelectedKeyword = keyword;
                    });
                },
                methods: {
                    updateSearch(keyword) {
                        if (keyword && keyword !== this.SelectedKeyword) {
                            this.SelectedKeyword = keyword;
                        self.eventHub.publish(SearchEvents.SearchKeywordChanged, {
                                data: {
                                    keyword
                                }
                            });
                        }
                    }
                }
            });
        }
    }
}
