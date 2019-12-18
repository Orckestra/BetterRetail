using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.Providers.MonerisPayment.ServiceModel;

namespace Orckestra.Composer.Cart.Repositories
{
    public interface IVaultProfileRepository
    {
        Task<VaultProfileCreationResult> AddCreditCardAsync(AddCreditCardParam param);
    }
}
