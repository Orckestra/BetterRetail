using System.Collections.Generic;
using System.Globalization;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Composer.Product.ViewModels;

namespace Orckestra.Composer.Product.Parameters
{
    public class GenerateKvaItemsParam
    {
        public IList<Lookup> ProductLookups { get; set; }
        public Overture.ServiceModel.Products.Product Product { get; set; }
        public Dictionary<string, object> SelectedKvas { get; set; }
        public ProductDefinition ProductDefinition { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public IList<VariantViewModel> ProductVariants { get; set; }

        public GenerateKvaItemsParam()
        {
            ProductLookups = new List<Lookup>();
            ProductVariants = new List<VariantViewModel>();
        }
    }
}