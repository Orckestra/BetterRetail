using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrderTemplatesViewModel : BaseViewModel
    {
        public RecurringOrderTemplatesViewModel()
        {
            RecurringOrderTemplateViewModelList = new List<RecurringOrderTemplateViewModel>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this view model is being fetched via AJAX.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

        public List<RecurringOrderTemplateViewModel> RecurringOrderTemplateViewModelList { get; set; }
    }
}
