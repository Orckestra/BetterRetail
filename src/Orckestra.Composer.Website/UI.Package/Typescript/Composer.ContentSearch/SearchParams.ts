/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    const PAGE_PARAM = 'page';

    export abstract class SearchParams {
        public static getSearchParams(): URLSearchParams {
           return new URLSearchParams(window.location.search);
        }

        private static getSearchQuery(params: URLSearchParams): string {
            return '?' + params.toString()
        }

        public static currentPage(): number {
            const params = this.getSearchParams();
            return parseInt(params.get(PAGE_PARAM)) || 1
        }

        public static toPage(page: string): string {
            const params = this.getSearchParams();
            params.set(PAGE_PARAM, page);
            return this.getSearchQuery(params)
        }

        public static nextPage(): string {
            const params = this.getSearchParams();
            let page = parseInt(params.get(PAGE_PARAM)) || 1;
            page += 1;
            params.set(PAGE_PARAM, page.toString());
            return this.getSearchQuery(params)
        }

        public static previousPage(): string {
            const params = this.getSearchParams();
            let page = parseInt(params.get(PAGE_PARAM)) || 1;
            if( page > 1)
                page -= 1;
            params.set(PAGE_PARAM, page.toString());
            return this.getSearchQuery(params)
        }
    }
}
