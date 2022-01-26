using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveLineItemsParam
    {
        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The culture for returned cart info
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The customer id to who belong the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }


        /// <summary>
        /// The descriptors of the LineItems to be removed
        /// Required
        /// </summary>
        public List<LineItemDescriptor> LineItems { get; set; }


        /// <summary>
        /// The name of the cart where to delete item
        /// Required
        /// </summary>
        public string CartName { get; set; }

        public RemoveLineItemsParam()
        {
            LineItems = new List<LineItemDescriptor>();
        }

        public class LineItemDescriptor
        {
            public Guid Id { get; set; }

            public string ProductId { get; set; }

            public string VariantId { get; set; }
        }
    }
}
