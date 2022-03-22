/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    const PAGE_PARAM = 'page';
    const SORT_BY_PARAM = 'sortBy';
    const SORT_DIRECTION_PARAM = 'sortDirection';

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

        public static changeSorting(sortBy, sortDirection): string {
            const params = this.getSearchParams();
            params.set(SORT_BY_PARAM, sortBy);
            params.set(SORT_DIRECTION_PARAM, sortDirection);
            return this.getSearchQuery(params)
        }

        public static getLastSegment(): string {
            return window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1);
        }

        public static pushState(query: string): void {
            window.history.pushState(window.history.state, "", window.location.pathname + query);
        }
    }
}
