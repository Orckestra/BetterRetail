using Orckestra.Composer.Configuration;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Utils
{
    public static class ProductHelper
    {

        static ProductHelper()
        {
        }

        public static string GetProductOrVariantDisplayName(Product prduct, Variant variant, CultureInfo culture)
        {
            if (variant != null)
            {
                return variant.DisplayName != null ? variant.DisplayName.GetLocalizedValue(culture.Name) : prduct.DisplayName.GetLocalizedValue(culture.Name);
            }
            else
            {
                return prduct.DisplayName.GetLocalizedValue(culture.Name);
            }
        }

        
    }
}
