using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public interface ITaxViewModelFactory
    {
        /// <summary>
        /// Gets a TaxViewModel from a List of Overture Tax objects.
        /// </summary>
        /// <param name="taxes"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        IEnumerable<TaxViewModel> CreateTaxViewModels(IEnumerable<Tax> taxes, CultureInfo cultureInfo);
    }
}
