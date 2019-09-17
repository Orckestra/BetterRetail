using Orckestra.Composer.Cart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Helper
{
    public class RecurringOrderCartViewModelNextOcurrenceComparer : IComparer<CartViewModel>
    {
        public int Compare(CartViewModel x, CartViewModel y)
        {
            var extendX = x.AsExtensionModel<IRecurringOrderCartViewModel>();
            var extendY = y.AsExtensionModel<IRecurringOrderCartViewModel>();

            return DateTime.Compare(extendX.NextOccurence, extendY.NextOccurence);

        }
    }
}
