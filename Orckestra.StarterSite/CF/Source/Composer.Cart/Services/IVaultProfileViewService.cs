using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    public interface IVaultProfileViewService
    {
        Task<MonerisAddVaultProfileViewModel> AddCreditCardAsync(AddCreditCardParam addCreditCardParam);
    }
}
