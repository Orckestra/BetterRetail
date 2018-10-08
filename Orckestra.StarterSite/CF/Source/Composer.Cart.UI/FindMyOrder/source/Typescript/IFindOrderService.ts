///<reference path='../../../../Composer.UI/Source/Typings/tsd.d.ts' />
///<reference path='./IGetOrderDetailsUrlRequest.ts' />
///<reference path='./IGuestOrderDetailsViewModel.ts' />

module Orckestra.Composer {
    export interface IFindOrderService {
        getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel>;
    }
}
