using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.LineItemValidation
{
    /// <summary>
    /// Line Item Validation Provider based on the implementation of the SurfaceLineItemsErrorsAsMessagesActivity in
    /// the Cart Workflow in Overture.
    /// </summary>
    public class SurfacedErrorLineItemValidationProvider : ILineItemValidationProvider
    {
        public const string LineItemIdKey = "LineItemId";
        public const string EntityTypeKey = "EntityType";
        public const string IsValidKey = "IsValid";
        public const string InStockLineItemStatus = "InStock";

        /// <summary>
        /// Determines if a line item is valid.
        /// </summary>
        /// <param name="cart">Cart to which belongs the line item.</param>
        /// <param name="lineItem">Line item to validate.</param>
        /// <returns></returns>
        public bool ValidateLineItem(ProcessedCart cart, LineItem lineItem)
        {
            var propertyBag = lineItem.PropertyBag ?? (lineItem.PropertyBag = new PropertyBag());
            var erronousLineItems = GetErronousLineItemMessages(cart);
            var group = erronousLineItems.FirstOrDefault(g => g.Key == lineItem.Id);
            var isValid = IsValid(lineItem, group);

            propertyBag[IsValidKey] = isValid;

            return isValid;
        }

        protected virtual bool IsValid(LineItem lineItem, IGrouping<Guid, ExecutionMessage> group)
        {
            var isValid = true;

            if (group != null &&
                (lineItem.Status == null || !lineItem.Status.Equals(InStockLineItemStatus, StringComparison.OrdinalIgnoreCase)))
            {
                AddErrorsToLineItem(lineItem, group);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Obtains a grouping of ExecutionMessage for each line item.
        /// </summary>
        /// <param name="cart">Processed Cart.</param>
        /// <returns></returns>
        protected virtual IEnumerable<IGrouping<Guid, ExecutionMessage>> GetErronousLineItemMessages(ProcessedCart cart)
        {
            if (cart.Messages == null) { return Enumerable.Empty<IGrouping<Guid, ExecutionMessage>>(); }

            var lineItems = cart.Messages
                                .Where(m => IsMessageForLineItem(m) && IsErrorMessage(m))
                                .GroupBy(m => Guid.Parse(m.PropertyBag[LineItemIdKey].ToString()));

            return lineItems;
        }

        protected bool IsMessageForLineItem(ExecutionMessage message)
        {
            var isForLineItem = message.PropertyBag.ContainsKey(EntityTypeKey)
                                && string.Equals(message.PropertyBag[EntityTypeKey].ToString(), "LineItem", StringComparison.InvariantCultureIgnoreCase)
                                && message.PropertyBag.ContainsKey(LineItemIdKey)
                                && !string.IsNullOrWhiteSpace(message.PropertyBag[LineItemIdKey].ToString());

            return isForLineItem;
        }

        protected bool IsErrorMessage(ExecutionMessage message)
        {
            var isError = message.Severity == ExecutionMessageSeverity.Error;
            return isError;
        }

        /// <summary>
        /// Provides an entry point when a line item is detected invalid to include more information.
        /// </summary>
        /// <param name="lineItem">LineItem that was detected invalid.</param>
        /// <param name="messages">Messages found for given LineItem.</param>
        protected virtual void AddErrorsToLineItem(LineItem lineItem, IEnumerable<ExecutionMessage> messages)
        {
            //Provided as hook for customers. Not actually used in the implementation.
        }
    }
}