using System.Globalization;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    public class FakeShippingTrackingProvider : IShippingTrackingProvider
    {
        public string ProviderName { get; set; }
        public string ProviderType { get; private set; }
        public TrackingInfoViewModel GetTrackingInfoViewModel(Shipment shipment, CultureInfo cultureInfo)
        {
           return new TrackingInfoViewModel();
        }
    }
}
