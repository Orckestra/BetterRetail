using Orckestra.Overture.ServiceModel.RecurringOrders;

namespace Orckestra.Composer.Extensions
{
    /// <summary>
    /// Extends the <see cref="RecurringOrderLineItem" />
    /// </summary>
    public static class RecurringOrderLineItemExtentions
    {
        /// <summary>
        /// Gets the line item image url
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>Image Url</returns>
        public static string GetImageUrl(this RecurringOrderLineItem lineItem)
        {
            return lineItem.PropertyBag != null && lineItem.PropertyBag.ContainsKey("ImageUrl") ? lineItem.PropertyBag["ImageUrl"].ToString() : null;
        }
    }
}
