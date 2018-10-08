using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    public class FakePaymentProvider : IPaymentProvider
    {
        public bool ShouldCapturePaymentValue { get; private set; }
        public string CapturePaymentUrlValue { get; private set; }

        public string ProviderName { get; set; }

        public string ProviderType { get; private set; }

        public FakePaymentProvider()
        {
            ShouldCapturePaymentValue = GetRandom.Boolean();
            CapturePaymentUrlValue = GetRandom.Url();
            ProviderType = GetRandom.String(15);
        }

        public Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(Overture.ServiceModel.Orders.Cart cart, InitializePaymentParam param)
        {
            return Task.FromResult(cart);
        }

        public bool ShouldCapturePayment(Payment payment)
        {
            return ShouldCapturePaymentValue;
        }

        public string GetCapturePaymentUrl(Payment payment, CultureInfo cultureInfo)
        {
            return CapturePaymentUrlValue;
        }

        public void AugmentViewModel(ActivePaymentViewModel viewModel, Payment payment)
        {
        }

        public OrderSummaryPaymentViewModel BuildOrderSummaryPaymentViewModel(Payment payment, CultureInfo cultureInfo)
        {
            var paymentVm = new OrderSummaryPaymentViewModel
            {
                FirstName = payment.BillingAddress.FirstName,
                LastName = payment.BillingAddress.LastName
            };

            paymentVm.BillingAddress = new AddressViewModel();
            paymentVm.Amount = payment.Amount.ToString();

            return paymentVm;
        }
    }
}