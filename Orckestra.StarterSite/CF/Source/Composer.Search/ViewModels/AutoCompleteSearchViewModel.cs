using Orckestra.Composer.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Search.ViewModels
{
    public class AutoCompleteSearchViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Query { get; set; }
    }
}