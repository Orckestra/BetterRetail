using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrderProgramViewModel : BaseViewModel
    {
        public bool IsActive { get; set; }
        public string RecurringOrderProgramName { get; set; }
        public string DisplayName { get; set; }
        public List<RecurringOrderProgramFrequencyViewModel> Frequencies { get; set; }

        public RecurringOrderProgramViewModel()
        {
            Frequencies = new List<RecurringOrderProgramFrequencyViewModel>();
        }
    }
}

