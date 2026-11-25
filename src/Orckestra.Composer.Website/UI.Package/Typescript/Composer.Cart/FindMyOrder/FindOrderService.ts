///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/IEventHub.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='./IFindOrderService.ts' />
///<reference path='./IGetOrderDetailsUrlRequest.ts' />
///<reference path='./IGuestOrderDetailsViewModel.ts' />

namespace Orckestra.Composer {
    export class FindOrderService implements IFindOrderService {
        private eventHub: IEventHub;

        public constructor(eventHub: IEventHub) {
            this.eventHub = eventHub;
        }

        public getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Promise<IGuestOrderDetailsViewModel> {
            var promise = ComposerClient.post('/api/order/url', req)
                .then((vm: IGuestOrderDetailsViewModel) => {
                    this.eventHub.publish('orderDetailsUrlUpdated', {
                        data: vm
                    });

                    return vm;
                });

            return promise;
        }

        public addOrderToCurrentUser(orderNumber: string): Promise<any> {
            return ComposerClient.post(`/api/order/attach-customer/${orderNumber}`, {});
        }
    }
}
