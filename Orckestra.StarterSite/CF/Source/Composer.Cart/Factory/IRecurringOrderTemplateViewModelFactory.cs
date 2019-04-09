using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Factory
{
    public interface IRecurringOrderTemplateViewModelFactory
    {
        /// <summary>
        /// Creates a <see cref="RecurringOrderTemplateViewModel" /> based on a <see cref="Template"/> object.
        /// </summary>
        /// <param name="param">Parameters used to create the ViewModel. May not be null.</param>
        /// <returns></returns>
        Task<RecurringOrderTemplatesViewModel> CreateRecurringOrderTemplatesViewModel(CreateRecurringOrderTemplatesViewModelParam param);

        /// <summary>
        /// Map the address of the client
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        AddressViewModel GetAddressViewModel(Address address, CultureInfo cultureInfo);

        /// <summary>
        /// Gets a ShippingMethodViewModel from an Overture FulfillmentMethodInfo object.
        /// </summary>
        /// <param name="fulfillmentMethodInfo"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        RecurringOrderShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethodInfo fulfillmentMethodInfo, CultureInfo cultureInfo);
    }
}

