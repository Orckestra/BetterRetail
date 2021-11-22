using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class SpecificationsGroupViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string GroupName { get; set; }

        public List<SpecificationsAttributeViewModel> Attributes { get; set; }

        public SpecificationsGroupViewModel()
        {
            Attributes = new List<SpecificationsAttributeViewModel>();
        }
    }
}