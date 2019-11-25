using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Parameters
{
    public class CreateProductDetailViewModelParam
    {
        public Overture.ServiceModel.Products.Product Product { get; set; }
        public ProductDefinition ProductDefinition { get; set; }
        public List<Lookup> ProductLookups { get; set; }
        public List<AllProductImages> ProductDetailImages { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public string VariantId { get; set; }
        public string BaseUrl { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public CreateProductDetailViewModelParam()
        {
            ProductLookups = new List<Lookup>();
            ProductDetailImages = new List<AllProductImages>();
        }
    }
}
