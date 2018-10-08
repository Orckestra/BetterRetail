using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    /// <summary>
    /// Product Specifications ViewModel. This class represents the custom attributes of a Product.
    /// The Attributes are divised in Groups.
    /// </summary>
    public sealed class SpecificationsViewModel : BaseViewModel
    {
        public string ProductId { get; set; }

        public string VariantId { get; set; }

        public List<SpecificationsGroupViewModel> Groups { get; set; }

        public SpecificationsViewModel()
        {
            Groups = new List<SpecificationsGroupViewModel>();
        }
    }
}
