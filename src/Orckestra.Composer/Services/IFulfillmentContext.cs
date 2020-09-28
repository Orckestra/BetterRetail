using System;

namespace Orckestra.Composer.Services
{
    /// <summary>
    /// Contains information about fulfillment.
    /// </summary>
    public interface IFulfillmentContext
    {
        /// <summary>
        /// The date on which product should be available and based on which effective product price should be calculated.
        /// </summary>
        DateTime? AvailabilityAndPriceDate { get; }
    }
}
