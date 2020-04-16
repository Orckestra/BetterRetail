using System.Globalization;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.ShippingTracking
{
    public class NullShippingTrackingProvider : IShippingTrackingProvider
    {
        public const string NullProviderTypeName = "NullShippingTrackingProvider";

        public string ProviderName { get; set; }
        public string ProviderType { get { return NullProviderTypeName; } }

        /// <summary>
        /// Gets a TrackingInfoViewModel.
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public TrackingInfoViewModel GetTrackingInfoViewModel(Shipment shipment, CultureInfo cultureInfo)
        {
            return null;
        }
    }
}
