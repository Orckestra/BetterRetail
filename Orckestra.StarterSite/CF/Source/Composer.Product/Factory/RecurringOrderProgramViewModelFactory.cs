using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.RecurringOrders;

namespace Orckestra.Composer.Product.Factory.Order
{
    public class RecurringOrderProgramViewModelFactory : IRecurringOrderProgramViewModelFactory
    {
        private readonly IViewModelMapper _viewModelMapper;
        private readonly ILocalizationProvider _localizationProvider;

        public RecurringOrderProgramViewModelFactory(
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider)
        {
            _viewModelMapper = viewModelMapper;
            _localizationProvider = localizationProvider;
        }

        public RecurringOrderProgramViewModel CreateRecurringOrderProgramViewModel(RecurringOrderProgram program, CultureInfo culture)
        {
            if (program == null) { throw new ArgumentNullException(nameof(program)); }
            if (culture == null) { throw new ArgumentNullException(nameof(culture)); }

            var vm = _viewModelMapper.MapTo<RecurringOrderProgramViewModel>(program, culture);

            if (vm == null) { return null; }

            var programLocalized = program.Localizations?.FirstOrDefault(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));

            if (programLocalized != null)
            {
                vm.DisplayName = programLocalized.DisplayName;

                foreach (var frequency in program.Frequencies ?? Enumerable.Empty<RecurringOrderFrequency>())
                {

                    var vmFrequency = vm.Frequencies.FirstOrDefault(f => string.Equals(f.RecurringOrderFrequencyName, frequency.RecurringOrderFrequencyName, StringComparison.OrdinalIgnoreCase));
                    if (vmFrequency != null)
                    {

                        var localizlocalizedFrequency = frequency.Localizations.FirstOrDefault(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));

                        if (localizlocalizedFrequency != null)
                        {
                            vmFrequency.DisplayName = localizlocalizedFrequency.DisplayName;
                        }
                        else
                        {
                            vmFrequency.DisplayName = vmFrequency.RecurringOrderFrequencyName;
                        }
                    }
                }
            }

            vm.Frequencies = vm.Frequencies.OrderBy(f => f.SequenceNumber).ToList();

            return vm;
        }
    }
}
