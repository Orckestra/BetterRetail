using System.Globalization;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.ShippingTracking
{
    public interface IShippingTrackingProvider
    {
        /// <summary>
        /// Name of the provider.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Type of Overture Payment Provider.
        /// </summary>
        string ProviderType { get; }


        /// <summary>
        /// Gets a TrackingInfoViewModel.
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        TrackingInfoViewModel GetTrackingInfoViewModel(Shipment shipment, CultureInfo cultureInfo);
    }
}
