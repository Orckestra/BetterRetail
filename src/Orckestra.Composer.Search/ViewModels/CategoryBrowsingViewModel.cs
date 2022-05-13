using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
    public sealed class CategoryBrowsingViewModel : BaseSearchViewModel
    {
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<string> LandingPageUrls { get; set; }
    }
}
