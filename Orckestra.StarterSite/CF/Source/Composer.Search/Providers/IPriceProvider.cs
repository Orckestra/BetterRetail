using System.Threading.Tasks;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Providers
{
    public interface IPriceProvider
    {
        Task<ProductPriceSearchViewModel> GetPriceAsync(
            bool? hasVariants, 
            ProductDocument document);
    }
}