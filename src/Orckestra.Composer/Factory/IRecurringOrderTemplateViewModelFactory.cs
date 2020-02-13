using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Factory
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
        /// Creates a <see cref="RecurringOrderTemplateViewModel" /> based on a <see cref="Template"/> object.
        /// </summary>
        /// <param name="param">Parameters used to create the ViewModel. May not be null.</param>
        /// <returns></returns>
        Task<RecurringOrderTemplateViewModel> CreateRecurringOrderTemplateDetailsViewModel(CreateRecurringOrderTemplateDetailsViewModelParam param);

        /// <summary>
        /// Group the recurring item template by shipping address
        /// </summary>
        /// <returns></returns>
        Task<List<RecurringOrderTemplateViewModel>> CreateTemplateGroupedShippingAddress(CreateTemplateGroupedShippingAddressParam param);

        /// <summary>
        /// Map the LineItem to the RecurringOrderLineItem
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<RecurringOrderTemplateLineItemViewModel> MapToTemplateLineItemViewModel(MapToTemplateLineItemViewModelParam param);


        /// <summary>
        /// Gets a ShippingMethodViewModel from an Overture FulfillmentMethodInfo object.
        /// </summary>
        /// <param name="fulfillmentMethodInfo"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        RecurringOrderShippingMethodViewModel GetShippingMethodViewModel(FulfillmentMethodInfo fulfillmentMethodInfo, CultureInfo cultureInfo);
    }
}

