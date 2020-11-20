using Orckestra.Composer.Country;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryStoreViewModelFactory: StoreViewModelFactory
    {
        public GroceryStoreViewModelFactory(ILocalizationProvider localizationProvider,
            IStoreUrlProvider storeUrlProvider,
            IViewModelMapper viewModelMapper,
            IStoreScheduleProvider storeScheduleProvider,
            ICountryService countryService,
            IGoogleMapsUrlProvider googleMapsUrlProvider): base(localizationProvider, storeUrlProvider, viewModelMapper, storeScheduleProvider, countryService, googleMapsUrlProvider)
        {

        }

        public override StoreViewModel CreateStoreViewModel(CreateStoreViewModelParam param)
        {
            var vm = base.CreateStoreViewModel(param);
            var extentedVm = vm.AsExtensionModel<IGroceryStoreViewModel>();
            extentedVm.PickUpSchedule = CreateScheduleViewModel(param.Store.PickUpSchedule, param);
            extentedVm.DeliverySchedule = CreateScheduleViewModel(param.Store.DeliverySchedule, param);

            return vm;
        }

        protected virtual StoreScheduleViewModel CreateScheduleViewModel(FulfillmentSchedule schedule, CreateStoreViewModelParam param)
        {
            if(schedule == null) { return null; }

            var model = new StoreScheduleViewModel();
            var storeNowTime = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(param.Store.FulfillmentLocation.TimeZone))
            {
                var storeTimeInfo = TimeZoneInfo.FindSystemTimeZoneById(param.Store.FulfillmentLocation.TimeZone);
                storeNowTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, storeTimeInfo);
            }

            model.OpeningHours = GetOpeningHours(schedule, param.CultureInfo, storeNowTime);
            model.OpeningHourExceptions = GetOpeningHourExceptions(schedule, param.CultureInfo, storeNowTime);

            var todayOpeningTimes = StoreScheduleProvider.GetOpeningTimes(schedule, storeNowTime).ToList();
            model.TodayOpeningTimes = todayOpeningTimes.Select(ot => GetScheduleIntervalViewModel(ot, param.CultureInfo)).ToList();

            model.IsOpenNow = IsTimeInIntervals(storeNowTime.TimeOfDay, todayOpeningTimes);

            return model;
        }
    }
}
