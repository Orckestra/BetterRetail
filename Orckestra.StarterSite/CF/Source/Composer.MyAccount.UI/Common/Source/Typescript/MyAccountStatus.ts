///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/Controller.ts' />
///<reference path='../../../../Composer.UI/Source/TypeScript/Mvc/IControllerActionContext.ts' />
///<reference path='../../../../Composer.UI/Source/Typescript/jqueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../../Common/Source/Typescript/MembershipService.ts' />

module Orckestra.Composer {

    export enum MyAccountStatus {
        Success,
        InvalidTicket,
        DuplicateEmail,
        DuplicateUserName,
        InvalidQuestion,
        InvalidPassword,
        InvalidPasswordAnswer,
        InvalidEmail,
        Failed,
        UserRejected,
        RequiresApproval,
        AjaxFailed
    };
}
