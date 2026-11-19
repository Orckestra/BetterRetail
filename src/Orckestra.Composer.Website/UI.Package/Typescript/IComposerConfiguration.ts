/// <reference path='./Mvc/IControllerConfiguration.ts' />

namespace Orckestra.Composer {
    export interface IComposerConfiguration {
        plugins?: string[];
        paymentProvider?: {
            name: string;
            origin: string;
            profileId: string;
        };
        controllers: IControllerConfiguration[];
    }
}
