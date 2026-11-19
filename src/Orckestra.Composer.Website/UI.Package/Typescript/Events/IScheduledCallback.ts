///<reference path='../../Typings/tsd.d.ts' />

namespace Orckestra.Composer {

    export interface IScheduledCallback {

        (data: any): Q.Promise<any>;
    }
}