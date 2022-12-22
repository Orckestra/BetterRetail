using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Extensions
{
    /// <summary>
    /// Extends the <see cref="LineItem" />
    /// </summary>
    public static class LineItemExtentions
    {
        /// <summary>
        /// Gets the line item image url
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>Image Url</returns>
        public static string GetImageUrl(this LineItem lineItem)
        {
            return lineItem.PropertyBag != null && lineItem.PropertyBag.ContainsKey("ImageUrl") ? lineItem.PropertyBag["ImageUrl"].ToString() : null;
        }
 
    }
}
