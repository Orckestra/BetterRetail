///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./ViewModels/IPaymentMethodViewModel.ts' />

namespace Orckestra.Composer {
    export interface IPaymentRepository {

        /**
         * Return a list of saved payment methods for a specified array of payment provider names
         * @param  {providers: Array<string>}       Array of provider names.
         * @return {Array<IPaymentMethodViewModel>} Instance of the provider.
         */
        getPaymentMethods(providers: Array<string>) : Promise<IPaymentViewModel>;

        /**
         * Return the active payment for the active cart
         * @return {IActivePaymentViewModel} Active payment for the active cart.
         */
        getActivePayment() : Promise<IActivePaymentViewModel>;

        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Promise<void>;

        setPaymentMethod(request : any) : Promise<IPaymentViewModel>;

        /**
         * Return a list of payment methods and payment provider
         * @return {ICheckoutPaymentViewModel} Instance of the provider.
         */
        getCheckoutPayment() : Promise<ICheckoutPaymentViewModel>;

        updatePaymentMethod(request : any) : Promise<IActivePaymentViewModel>;
    }
}
