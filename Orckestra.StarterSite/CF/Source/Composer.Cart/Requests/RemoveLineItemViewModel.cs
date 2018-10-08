using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Requests
{
    public sealed class RemoveLineItemViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string LineItemId { get; set; }
    }
}
