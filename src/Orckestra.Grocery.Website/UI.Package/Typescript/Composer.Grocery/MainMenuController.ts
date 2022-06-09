/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typings/vue/index.d.ts' />
/// <reference path='../Utils/UrlHelper.ts' />
///<reference path='../Composer.MyAccount/Common/IMembershipService.ts' />
///<reference path='../Composer.MyAccount/Common/MembershipService.ts' />

module Orckestra.Composer {
    'use strict';

    export class MainMenuController extends Orckestra.Composer.Controller {
        protected vueMainMenu: Vue;
        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {
            super.initialize();

            this.membershipService.isAuthenticated()
                .then((authVm) => {
                    if(authVm.IsAuthenticated) {
                       this.context.container.addClass("is-authorized");
                    }
                })
        }
    }
}
