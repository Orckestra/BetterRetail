using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.MyAccount.Factory
{
    public interface ICustomerViewModelFactory
    {      
        /// <summary>
        /// Gets a PaymentMethodViewModel from an Overture PaymentMethod object.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <param name="paymentMethodDisplayNames"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        ICustomerPaymentMethodViewModel GetPaymentMethodViewModel(PaymentMethod paymentMethod, Dictionary<string, string> paymentMethodDisplayNames, CultureInfo cultureInfo);   
    }
}
