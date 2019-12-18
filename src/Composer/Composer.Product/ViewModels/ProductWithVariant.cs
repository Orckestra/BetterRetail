using Orckestra.Composer.Product.Services;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.ViewModels
{
    /// <summary>
    /// This is a small shim class to make it easier for 
    /// <see cref="BaseProductViewService{TParam}.GetRelatedProductsAsync"/>
    /// to return objects with a clear, single variant specified.
    /// </summary>
    public class ProductWithVariant
    {
        public Overture.ServiceModel.Products.Product Product { get; set; }
        public Variant Variant { get; set; }
    }
}