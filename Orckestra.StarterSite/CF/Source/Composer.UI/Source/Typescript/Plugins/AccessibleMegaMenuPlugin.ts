/// <reference path='./IPlugin.ts' />
/// <reference path='../JQueryPlugins/IAccessibleMegaMenuJqueryPlugin.ts' />

module Orckestra.Composer {
    export class AccessibleMegaMenuPlugin implements IPlugin {
        public initialize() {
            (<Orckestra.Composer.IAccessibleMegaMenuJqueryPlugin>$('.megamenu')).accessibleMegaMenu();
        }
    }
}
