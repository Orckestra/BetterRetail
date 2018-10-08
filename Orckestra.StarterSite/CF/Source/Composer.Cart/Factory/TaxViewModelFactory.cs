using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public class TaxViewModelFactory : ITaxViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public TaxViewModelFactory(IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }

            ViewModelMapper = viewModelMapper;
            LocalizationProvider = localizationProvider;
        }

        public virtual IEnumerable<TaxViewModel> CreateTaxViewModels(IEnumerable<Tax> taxes, CultureInfo cultureInfo)
        {
            if (taxes == null)
            {
                yield break;
            }

            var taxesByCodes = taxes.Where(x=>x.TaxTotal.HasValue && x.TaxTotal.Value > 0).GroupBy(t => t.DisplayName);

            foreach (var codeTaxes in taxesByCodes)
            {
                var vm = ViewModelMapper.MapTo<TaxViewModel>(codeTaxes.First(), cultureInfo);

                vm.TaxTotal = codeTaxes.Sum(t => t.TaxTotal);
                vm.DisplayTaxTotal = vm.TaxTotal.HasValue
                    ? LocalizationProvider.FormatPrice(vm.TaxTotal.Value, cultureInfo)
                    : string.Empty;

                yield return vm;
            }
        }
    }
}
