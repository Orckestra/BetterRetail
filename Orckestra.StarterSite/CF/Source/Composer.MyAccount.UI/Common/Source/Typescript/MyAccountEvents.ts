///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/jqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../../Common/Source/Typescript/MembershipService.ts' />

module Orckestra.Composer {

    export enum MyAccountEvents {
        AccountCreated,
        AccountUpdated,
        AddressCreated,
        AddressUpdated,
        AddressDeleted,
        LoggedIn,
        LoggedOut,
        PasswordChanged,
        ForgotPasswordInstructionSent
    }
}
