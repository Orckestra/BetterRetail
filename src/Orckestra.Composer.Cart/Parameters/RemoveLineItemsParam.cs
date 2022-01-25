using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveLineItemsParam : BaseCartParam
    {
        /// <summary>
        /// The descriptors of the LineItems to be removed
        /// Required
        /// </summary>
        public List<LineItemDescriptor> LineItems { get; set; }

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