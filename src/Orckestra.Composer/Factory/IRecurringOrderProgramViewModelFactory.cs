using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Factory
{
    public interface IRecurringOrderProgramViewModelFactory
    {
        RecurringOrderProgramViewModel CreateRecurringOrderProgramViewModel(RecurringOrderProgram program, CultureInfo culture);
    }
}
