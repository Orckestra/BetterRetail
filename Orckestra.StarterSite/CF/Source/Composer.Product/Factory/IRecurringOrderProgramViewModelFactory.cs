using Orckestra.Composer.Product.ViewModels;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Factory.Order
{
    public interface IRecurringOrderProgramViewModelFactory
    {
        RecurringOrderProgramViewModel CreateRecurringOrderProgramViewModel(RecurringOrderProgram program, CultureInfo culture);
    }
}
