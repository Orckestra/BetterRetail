using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Cart.Services
{
    public class LineItemService : ILineItemService
    {
        protected IDamProvider DamProvider { get; private set; }
        public ILineItemValidationProvider LineItemValidationProvider { get; set; }

        public LineItemService(IDamProvider damProvider, ILineItemValidationProvider lineItemValidationProvider)
        {
            if (damProvider == null) { throw new ArgumentNullException("damProvider"); }
            if (lineItemValidationProvider == null) { throw new ArgumentNullException("lineItemValidationProvider"); }

            DamProvider = damProvider;
            LineItemValidationProvider = lineItemValidationProvider;
        }

        public virtual List<LineItem> GetInvalidLineItems(ProcessedCart cart)
        {
            if (cart == null) { throw new ArgumentNullException("cart"); }

            var lineItems = cart.GetLineItems();

            var invalidLineItems = lineItems.Where(lineItem => !LineItemValidationProvider.ValidateLineItem(cart, lineItem)).ToList();
            return invalidLineItems;
        }

      
    }
}
