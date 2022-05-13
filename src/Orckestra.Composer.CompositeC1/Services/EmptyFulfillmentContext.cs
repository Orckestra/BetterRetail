using System;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Services
{
    /// <summary>
    /// Default implementation of fulfillment context.
    /// </summary>
    public class EmptyFulfillmentContext: IFulfillmentContext
    {
        /// <summary>
        /// The date on which product should be available, and based on which effective price should be calculated.
        /// </summary>
        public DateTime? AvailabilityAndPriceDate => null;
    }
}
