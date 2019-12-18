using Orckestra.Composer.ViewModels.MyAccount;

namespace Orckestra.Composer.Services
{
    public interface IMyAccountViewService
    {
        MenuViewModel CreateMenu(string currentUrl);
    }
}
