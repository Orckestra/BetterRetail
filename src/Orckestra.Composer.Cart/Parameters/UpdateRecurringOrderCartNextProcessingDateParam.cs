using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateRecurringOrderCartNextOccurenceParam : BaseCartParam
    {
        public DateTime NextOccurence { get; set; }
        public string BaseUrl { get; set; }
    }
}