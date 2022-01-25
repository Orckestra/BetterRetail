using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCustomerPaymentMethodListViewModelParam : BaseCartParam
    {
        public List<string> ProviderNames { get; set; }
    }
}
