using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Parameters
{
    public class CreateRelatedProductViewModelParam
    {
        public IEnumerable<ProductWithVariant> ProductsWithVariant { get; set; }
        public Uri BaseUrl { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string Scope { get; set; }
        public List<ProductPrice> Prices { get; set; }
        public List<ProductMainImage> Images { get; set; }

        public CreateRelatedProductViewModelParam()
        {
            Prices = new List<ProductPrice>();  
            Images = new List<ProductMainImage>();
        }
    }
}
