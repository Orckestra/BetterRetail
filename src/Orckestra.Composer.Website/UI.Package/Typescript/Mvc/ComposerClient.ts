///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./Localization/ILocalizationProvider.ts' />
///<reference path='./Localization/LocalizationProvider.ts' />

module Orckestra.Composer {
    'use strict';

    interface EnhancedJQueryAjaxSettings extends JQueryAjaxSettings {
        method: string;
    }

    export class ComposerClient {

        public static get(url: string, data?: any) : Q.Promise<any> {

            return this.sendRequest('GET', url, data);
        }

        public static post(url: string, data: any) : Q.Promise<any> {

            return this.sendRequest('POST', url, data ? JSON.stringify(data) : null);
        }

        public static put(url: string, data: any) : Q.Promise<any> {

            return this.sendRequest('PUT', url, data ? JSON.stringify(data) : null);
        }

        public static remove(url: string, data: any) : Q.Promise<any> {

            return this.sendRequest('DELETE', url, data ? JSON.stringify(data) : null);
        }

        private static sendRequest(method: string, url: string, data?: any) : Q.Promise<any> {

            const settings: EnhancedJQueryAjaxSettings = {
                contentType: 'application/json',
                dataType: 'json',
                data,
                method,
                url,
                headers: {
                    'Accept-Language': this.getPageCulture(),
                    'WebsiteId': this.getWebsiteId()
                }
            };

            return Q($.ajax(settings)).fail(reason => this.onRequestRejected(reason));
        }

        private static getPageCulture(): string {

            var culture: string = $('html').attr('lang');
            if (!culture) {
                throw new Error('No lang attribute was found on the <html> element. Please make sure it is included.');
            }

            return culture;
        }

        private static getWebsiteId(): string {

            var websiteId: string = $('html').data('website');
            if (!websiteId) {
                throw new Error('No websiteId was found on the <html> element. Please make sure it is included.');
            }

            return websiteId;
        }

        private static onRequestRejected(reason: any): void {

            if (reason.readyState === 0) {
                throw { Errors: [ { LocalizedErrorMessage: this.getAjaxFailedErrorMessage() } ] };
            }

            if (reason.readyState === 4 && reason.status === 205) {
                console.log('Page must be reloaded.');
                var redirectUrl = this.getReloadUrl();
                window.location.href = redirectUrl;
            }

            if (reason.readyState === 4 && reason.status === 401) {
                throw { Errors: [ { LocalizedErrorMessage: this.getUnauthorizedErrorMessage() } ] };
            }

            if (reason.readyState === 4 && reason.status === 500 && reason.responseJSON !== null) {
                throw { Errors: reason.responseJSON.Errors || reason.responseJSON.ExceptionMessage };
            }

            throw reason;
        }

        private static getReloadUrl(): string {
            var rawQueryString: string = window.location.search;

            var value: string = 'session=expired';

            if (this.doesUrlContainQueryString(rawQueryString, value)) {
                return window.location.href;
            }

            var splitter = '&';
            if (_.isEmpty(rawQueryString)) {
                splitter = '/?';
            }

            var qs: string = `${rawQueryString}${splitter}${value}`;

            var urls: string[] = window.location.href.split('?', 2);
            var url: string = `${urls[0]}${qs}`;

            return url;
        }

        private static doesUrlContainQueryString(url: string, value: string): boolean {
            var regex: RegExp = new RegExp('(\\?|\\&)' + value, 'i');

            return regex.test(url);
        }

        private static getAjaxFailedErrorMessage(): string {

            return LocalizationProvider.instance().getLocalizedString('General', 'L_ErrorAjaxFailed');
        }

        private static getUnauthorizedErrorMessage(): string {

            return LocalizationProvider.instance().getLocalizedString('General', 'L_ErrorUnauthorized');
        }
    }
}
