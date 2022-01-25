using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveRecurringCartLineItemParam : BaseCartParam
    {
        public Guid LineItemId { get; set; }
        public string BaseUrl { get; set; }
    }
}