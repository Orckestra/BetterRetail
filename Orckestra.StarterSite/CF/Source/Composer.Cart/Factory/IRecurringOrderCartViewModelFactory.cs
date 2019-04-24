using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Factory
{
    public interface IRecurringOrderCartViewModelFactory
    {
        /// <summary>
        /// Creates a <see cref="IRecurringOrderCartViewModel" /> based on a <see cref="Cart"/> object.
        /// </summary>
        /// <param name="param">Parameters used to create the ViewModel. May not be null.</param>
        /// <returns></returns>
        IRecurringOrderCartViewModel CreateRecurringOrderCartViewModel(CreateRecurringOrderCartViewModelParam param);

        /// <summary>
        /// Creates a <see cref="LightRecurringOrderCartViewModel" /> based on a <see cref="Cart"/> object.
        /// </summary>
        /// <param name="param">Parameters used to create the ViewModel. May not be null.</param>
        /// <returns></returns>
        LightRecurringOrderCartViewModel CreateLightRecurringOrderCartViewModel(CreateLightRecurringOrderCartViewModelParam param);
    }
}
