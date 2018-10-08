using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.LineItemValidation
{
    public interface ILineItemValidationProvider
    {
        /// <summary>
        /// Determines if a line item is valid.
        /// </summary>
        /// <param name="cart">Cart to which belongs the line item.</param>
        /// <param name="lineItem">Line item to validate.</param>
        /// <returns></returns>
        bool ValidateLineItem(ProcessedCart cart, LineItem lineItem);
    }
}
