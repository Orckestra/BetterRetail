///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class LanguageSwitchController extends Controller {

        private languageSwitchEvent: string = 'languageSwitchEvent';
        private cacheProvider: ICacheProvider;
        public VueLanguageSwitch: Vue;
        public VueLanguageSwitchMobile: Vue;

        public initialize() {
            super.initialize();
            this.cacheProvider = CacheProvider.instance();
            let self: LanguageSwitchController = this;
            this.VueLanguageSwitch = new Vue({
                el: '#vueLanguageSwitch',
                methods: {
                    onLanguageSwitch() {
                        self.onLanguageSwitch();
                    }
                }
            });
            this.VueLanguageSwitchMobile = new Vue({
                el: '#vueLanguageSwitchMobile',
                methods: {
                    onLanguageSwitch() {
                        self.onLanguageSwitch();
                    }
                }
            });
        }

        public onLanguageSwitch() {
            this.cacheProvider.defaultCache.set(this.languageSwitchEvent, true);
        }
    }

}