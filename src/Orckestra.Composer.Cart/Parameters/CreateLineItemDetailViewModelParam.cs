using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CreateLineItemDetailViewModelParam
    {
        public Action<LineItem> PreMapAction { get; set; }

        public LineItem LineItem { get; set; }

        public IDictionary<(string ProductId, string VariantId), ProductMainImage> ImageDictionary { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string BaseUrl { get; set; }
    }
}
