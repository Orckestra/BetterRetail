namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartShippingMethodParam : BaseCartParam
    {
        public string ShippingProviderId { get; set; }
        public string ShippingMethodName { get; set; }
        public string BaseUrl { get; set; }
    }
}