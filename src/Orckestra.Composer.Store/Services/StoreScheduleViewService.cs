using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;

using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Store.Services
{
    public class StoreScheduleViewService : IStoreScheduleViewService
    {
        protected IStoreRepository StoreRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IStoreScheduleProvider StoreScheduleProvider { get; private set; }

        public StoreScheduleViewService(
           IStoreRepository storeRepository,
           IViewModelMapper viewModelMapper,
           IStoreScheduleProvider storeScheduleProvider)
        {
            StoreRepository = storeRepository;
            ViewModelMapper = viewModelMapper;
            StoreScheduleProvider = storeScheduleProvider;
        }
        public virtual async Task<StoreScheduleViewModel> GetStoreScheduleViewModelAsync(GetStoreScheduleParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.FulfillmentLocationId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param)); }

            var overtureSchedule = await StoreRepository.GetStoreScheduleAsync(param).ConfigureAwait(false);
            var model = ViewModelMapper.MapTo<StoreScheduleViewModel>(overtureSchedule, param.CultureInfo);

            var today = DateTime.Today;
            var now = DateTime.Now;

            var todayOpeningTimes = StoreScheduleProvider.GetOpeningTimes(overtureSchedule, today).ToList();
            model.IsOpenNow = IsTimeInIntervals(now.TimeOfDay, todayOpeningTimes);
            model.TodayOpeningTimes = todayOpeningTimes.Select(
                openingTime => ViewModelMapper.MapTo<ScheduleIntervalViewModel>(openingTime, param.CultureInfo)
            ).ToList();

            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(overtureSchedule, today, 1);
            model.OpeningHourExceptions =
                exceptions.Select(ex => ViewModelMapper.MapTo<DailyScheduleExceptionViewModel>(ex, param.CultureInfo))
                    .ToList();

            return model;
        }


        protected virtual bool IsTimeInIntervals(TimeSpan time, IEnumerable<ScheduleInterval> intervals)
        {
            return intervals.Select(interval => time >= interval.BeginingTime && time < interval.EndingTime).FirstOrDefault();
        }
    }
}
