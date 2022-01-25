namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringCartLineItemParam : BaseCartParam
    {
        public string LineItemId { get; set; }
        public double Quantity { get; set; }
        public string BaseUrl { get; set; }
    }
}
