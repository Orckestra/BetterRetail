using System.Globalization;
using Orckestra.Composer.Cart.Providers.Payment;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetActivePaymentViewModelParam
    {
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }

        public IPaymentProvider PaymentProvider { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
