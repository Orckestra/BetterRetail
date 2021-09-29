using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Parameters
{
    public class GetRelatedProductsParam
    {
        public CultureInfo CultureInfo { get; set; }
        public IEnumerable<ProductIdentifier> ProductIds { get; set; }
        public string Scope { get; set; }
        public Uri BaseUrl { get; set; }
        public string CurrencyIso { get; set; }
    }
}