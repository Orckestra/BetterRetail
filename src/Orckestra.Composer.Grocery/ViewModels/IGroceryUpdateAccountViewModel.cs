using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryUpdateAccountViewModel : IExtensionOf<UpdateAccountViewModel>
    {
        Dictionary<string, string> SubstitutionOptions { get; set; }
    }
}
