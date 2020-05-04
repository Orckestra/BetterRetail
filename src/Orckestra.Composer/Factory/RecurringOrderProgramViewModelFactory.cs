using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Providers;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.RecurringOrders;

namespace Orckestra.Composer.Factory
{
    public class RecurringOrderProgramViewModelFactory : IRecurringOrderProgramViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public RecurringOrderProgramViewModelFactory(
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider)
        {
            ViewModelMapper = viewModelMapper;
            LocalizationProvider = localizationProvider;
        }

        public virtual RecurringOrderProgramViewModel CreateRecurringOrderProgramViewModel(RecurringOrderProgram program, CultureInfo culture)
        {
            if (program == null) { throw new ArgumentNullException(nameof(program)); }
            if (culture == null) { throw new ArgumentNullException(nameof(culture)); }

            var vm = ViewModelMapper.MapTo<RecurringOrderProgramViewModel>(program, culture);

            if (vm == null) { return null; }

            var programLocalized = program.Localizations?.Find(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));

            if (programLocalized != null)
            {
                vm.DisplayName = programLocalized.DisplayName;

                if (program.Frequencies != null && program.Frequencies.Any())
                {
                    var dictionary = new Dictionary<string, RecurringOrderProgramFrequencyViewModel>(StringComparer.OrdinalIgnoreCase);
                    foreach (var vmFrequency in vm.Frequencies)
                    {
                        if (dictionary.ContainsKey(vmFrequency.RecurringOrderFrequencyName)) { continue; }
                        dictionary.Add(vmFrequency.RecurringOrderFrequencyName, vmFrequency);
                    }

                    foreach(var frequency in program.Frequencies)
                    {
                        dictionary.TryGetValue(frequency.RecurringOrderFrequencyName, out RecurringOrderProgramFrequencyViewModel vmFrequency);
                        if (vmFrequency != null)
                        {
                            var localizlocalizedFrequency = frequency.Localizations.Find(l => string.Equals(l.CultureIso, culture.Name, StringComparison.OrdinalIgnoreCase));
                            vmFrequency.DisplayName = localizlocalizedFrequency != null 
                                ? localizlocalizedFrequency.DisplayName 
                                : vmFrequency.RecurringOrderFrequencyName;
                        }
                    }
                }
            }
            vm.Frequencies = vm.Frequencies.OrderBy(f => f.SequenceNumber).ToList();
            return vm;
        }
    }
}