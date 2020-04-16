using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrderProgramFrequencyViewModel : BaseViewModel
    {
        public int NumberOfDays { get; set; }
        public string RecurringOrderFrequencyName { get; set; }
        public string DisplayName { get; set; }
        public int SequenceNumber { get; set; }
    }
}
