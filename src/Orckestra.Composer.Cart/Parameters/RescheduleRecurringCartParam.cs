using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RescheduleRecurringCartParam : BaseCartParam
    {
        public DateTime NextOccurence { get; set; }
    }
}
