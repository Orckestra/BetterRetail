using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Providers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    public class LineItemService : ILineItemService
    {
        protected IDamProvider DamProvider { get; private set; }
        public ILineItemValidationProvider LineItemValidationProvider { get; set; }

        public LineItemService(IDamProvider damProvider, ILineItemValidationProvider lineItemValidationProvider)
        {
            DamProvider = damProvider ?? throw new ArgumentNullException(nameof(damProvider));
            LineItemValidationProvider = lineItemValidationProvider ?? throw new ArgumentNullException(nameof(lineItemValidationProvider));
        }

        public virtual List<LineItem> GetInvalidLineItems(ProcessedCart cart)
        {
            if (cart == null) { throw new ArgumentNullException(nameof(cart)); }

            var lineItems = cart.GetLineItems();

            var invalidLineItems = lineItems.Where(lineItem => !LineItemValidationProvider.ValidateLineItem(cart, lineItem)).ToList();
            return invalidLineItems;
        }    
    }
}