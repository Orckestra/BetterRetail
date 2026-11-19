///<reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {
    export interface IRegisterActionOptions {
        actionName: string;
        actionDelegate: Function;
        overwrite?: boolean;
    }
}
