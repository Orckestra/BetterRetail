using System.Globalization;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Parameters
{
    public class CreateCategoryViewModelParam
    {
        public Category Category { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public CreateCategoryViewModelParam(Category category, CultureInfo cultureInfo)
        {
            Category = category;
            CultureInfo = cultureInfo;
        }
    }
}
