///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./IGetOrderDetailsUrlRequest.ts' />
///<reference path='./IGuestOrderDetailsViewModel.ts' />

namespace Orckestra.Composer {
    export interface IFindOrderService {
        getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Promise<IGuestOrderDetailsViewModel>;

        addOrderToCurrentUser(orderNumber: string): Promise<any>;
    }
}
