using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    /// <summary>
    /// A ViewModel class to hold a list of <see cref="RelatedProductViewModel"/>. 
    /// The main reson this class exists is to give a named property for Handlebars views to bind to.
    /// </summary>
    public sealed class RelatedProductsViewModel : BaseViewModel
    {
        public RelatedProductsViewModel()
        {
            Products = new List<RelatedProductViewModel>();
        }

        public bool DisplayPrices { get; set; }
        public bool DisplayAddToCart { get; set; }
        public string HeadingComponentText { get; set; }
        public string BackgroundStyle { get; set; }

        public IList<RelatedProductViewModel> Products { get; set; }
    }
}