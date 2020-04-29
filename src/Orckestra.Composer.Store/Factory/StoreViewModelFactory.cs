using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Utils;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.Store.Factory
{
    public class StoreViewModelFactory : IStoreViewModelFactory
    {
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }
        protected IStoreScheduleProvider StoreScheduleProvider { get; private set; }
        protected ICountryService CountryService { get; private set; }

        protected IGoogleMapsUrlProvider GoogleMaps { get; private set; }

        public StoreViewModelFactory(
            ILocalizationProvider localizationProvider,
            IStoreUrlProvider storeUrlProvider,
            IViewModelMapper viewModelMapper,
            IStoreScheduleProvider storeScheduleProvider,
            ICountryService countryService,
            IGoogleMapsUrlProvider googleMapsUrlProvider)
        {
            LocalizationProvider = localizationProvider;
            StoreUrlProvider = storeUrlProvider;
            ViewModelMapper = viewModelMapper;
            StoreScheduleProvider = storeScheduleProvider;
            CountryService = countryService;
            GoogleMaps = googleMapsUrlProvider;
        }

        public virtual StoreViewModel CreateStoreViewModel(CreateStoreViewModelParam param)
        {
            var store = param.Store;
            var storeViewModel = ViewModelMapper.MapTo<StoreViewModel>(store, param.CultureInfo);

            storeViewModel.Address = CreateStoreAddressViewModel(param);
            storeViewModel.LocalizedDisplayName = GetStoreLocalizedDisplayName(storeViewModel, param.CultureInfo);
            storeViewModel.FulfillmentLocationId = store.FulfillmentLocation.Id;
            storeViewModel.GoogleDirectionsLink = GetGoogleDirectionsLink(storeViewModel.Address);
            storeViewModel.GoogleStaticMapUrl = GetGoogleStaticMapUrl(storeViewModel.Address);

            storeViewModel.Url = StoreUrlProvider.GetStoreUrl(new GetStoreUrlParam()
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                StoreNumber = store.Number,
                StoreName = storeViewModel.Name
            });

            if (param.SearchPoint != null && storeViewModel.Address.Latitude != null &&
                storeViewModel.Address.Longitude != null)
            {
                storeViewModel.DestinationToSearchPoint = Math.Round(GeoCodeCalculator.CalcDistance(
                    storeViewModel.Address.Latitude.Value, storeViewModel.Address.Longitude.Value,
                    param.SearchPoint.Lat, param.SearchPoint.Lng, EarthRadiusMeasurement.Kilometers), 2);
            }

            storeViewModel.Schedule = CreateStoreScheduleViewModel(param);

            return storeViewModel;
        }

        public virtual StoreStructuredDataViewModel CreateStoreStructuredDataViewModel(CreateStoreViewModelParam param)
        {
            var store = param.Store;
            var model = new StoreStructuredDataViewModel
            {
                Name = store.Name,
                Telephone = store.PhoneNumber,
                Url = StoreUrlProvider.GetStoreUrl(new GetStoreUrlParam()
                {
                    BaseUrl = param.BaseUrl,
                    CultureInfo = param.CultureInfo,
                    StoreNumber = store.Number,
                    StoreName = store.Name
                })
            };
            var address = store.FulfillmentLocation?.Addresses?.FirstOrDefault();
            if (address != null)
            {
                model.StreetAddress = address.Line1;
                model.AddressLocality = address.City;
                model.AddressRegion = address.RegionCode;
                model.AddressCountry = address.CountryCode;
                model.PostalCode = address.PostalCode;
                model.Longitude = address.Longitude;
                model.Latitude = address.Latitude;
                if (string.IsNullOrWhiteSpace(model.Telephone))
                {
                    model.Telephone = address.PhoneNumber;
                }
            }

            if (store.StoreSchedule != null)
            {
                //// fixed datetime formats based on https://developers.google.com/structured-data/local-businesses/
                var dataTimeFormat = @"hh\:mm\:ss";
                var dateFormat = @"yyyy-MM-dd";
                model.OpeningHoursSpecifications = new List<StructuredDataOpeningHoursSpecificationViewModel>();
                var groups = store.StoreSchedule.OpeningHours
                    .Where(it => !it.IsClosed)
                    .GroupBy(
                        x =>
                            new
                            {
                                x.OpeningTimes.FirstOrDefault()?.BeginingTime,
                                x.OpeningTimes.FirstOrDefault()?.EndingTime
                            });
                foreach (var group in groups)
                {
                    var opens = group.Key.BeginingTime ?? DateTime.Today.TimeOfDay;
                    var closes = group.Key.EndingTime ?? DateTime.Today.AddTicks(-1).AddDays(1).TimeOfDay;
                    var openHourSpec = new StructuredDataOpeningHoursSpecificationViewModel
                    {
                        Opens = opens.ToString(dataTimeFormat),
                        Closes = closes.ToString(dataTimeFormat),
                        DayOfWeeks = new List<DayOfWeek>()
                    };
                    foreach (var item in group)
                    {
                        openHourSpec.DayOfWeeks.Add(item.Day);
                    }
                    model.OpeningHoursSpecifications.Add(openHourSpec);
                }

                var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(store.StoreSchedule, DateTime.Today, 1);
                foreach (var item in exceptions)
                {
                    var opens = item.IsClosed ? DateTime.Today.TimeOfDay : item.OpeningTime.BeginingTime;
                    var closes = item.IsClosed ? DateTime.Today.AddSeconds(1).TimeOfDay : item.OpeningTime.EndingTime;
                    var openHourSpec = new StructuredDataOpeningHoursSpecificationViewModel
                    {
                        ValidFrom = item.StartDate.ToString(dateFormat),
                        ValidThrough = item.EndDate.ToString(dateFormat),
                        Opens = opens.ToString(dataTimeFormat),
                        Closes = closes.ToString(dataTimeFormat),
                    };
                    model.OpeningHoursSpecifications.Add(openHourSpec);
                }
            }
            return model;
        }

        public virtual StorePageViewModel BuildNextPage(GetStorePageViewModelParam param)
        {
            return param.CurrentPageNumber < GetTotalPages(param.Total, param.PageSize)
                ? new StorePageViewModel
                {
                    Page = param.CurrentPageNumber + 1
                }
                : null;
        }

        protected virtual StoreScheduleViewModel CreateStoreScheduleViewModel(CreateStoreViewModelParam param)
        {
            if (param.Store.StoreSchedule == null) { return null; }

            var model = new StoreScheduleViewModel();
            var storeNowTime = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(param.Store.FulfillmentLocation.TimeZone))
            {
                var storeTimeInfo = TimeZoneInfo.FindSystemTimeZoneById(param.Store.FulfillmentLocation.TimeZone);
                storeNowTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, storeTimeInfo);
            }

            model.OpeningHours = GetOpeningHours(param, storeNowTime);
            model.OpeningHourExceptions = GetOpeningHourExceptions(param, storeNowTime);

            var todayOpeningTimes = StoreScheduleProvider.GetOpeningTimes(param.Store.StoreSchedule, storeNowTime).ToList();
            model.TodayOpeningTimes = todayOpeningTimes.Select(ot => GetScheduleIntervalViewModel(ot, param.CultureInfo)).ToList();

            model.IsOpenNow = IsTimeInIntervals(storeNowTime.TimeOfDay, todayOpeningTimes);

            return model;
        }

        protected virtual List<DailyScheduleExceptionViewModel> GetOpeningHourExceptions(CreateStoreViewModelParam param, DateTime today)
        {
            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(param.Store.StoreSchedule, today, 1);

            return exceptions.Select(
                ex => ViewModelMapper.MapTo<DailyScheduleExceptionViewModel>(new
                {
                    ex.StartDate,
                    ex.EndDate,
                    ex.IsClosed,
                    OpeningTime = GetScheduleIntervalViewModel(ex.OpeningTime, param.CultureInfo)
                }, param.CultureInfo))
                .ToList();
        }

        protected virtual List<DailyScheduleViewModel> GetOpeningHours(CreateStoreViewModelParam param, DateTime today)
        {
            return param.Store.StoreSchedule.OpeningHours.Select(oh =>
                new DailyScheduleViewModel
                {
                    LocalizedDay = GetStoreOpenHoursLocalizedDayName(oh.Day, param.CultureInfo),
                    IsDayToday = oh.Day == today.DayOfWeek,
                    IsClosed = oh.IsClosed,
                    IsOpenedAllDay = oh.IsOpenedAllDay,
                    OpeningTimes = oh.OpeningTimes.Select(ot => GetScheduleIntervalViewModel(ot, param.CultureInfo)).ToList()
                }
                ).ToList();
        }

        protected virtual string GetStoreOpenHoursLocalizedDayName(DayOfWeek day, CultureInfo cultureInfo)
        {
            return cultureInfo.DateTimeFormat.GetDayName(day);
        }

        protected virtual string GetOpeningTimeFormat(CultureInfo cultureInfo)
        {
            var openingTimeFormat = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Store",
                Key = "OpeningHourFormat",
                CultureInfo = cultureInfo
            });

            return string.IsNullOrWhiteSpace(openingTimeFormat) ? "{0:t}" : openingTimeFormat;
        }

        protected virtual ScheduleIntervalViewModel  GetScheduleIntervalViewModel(ScheduleInterval openingTime, CultureInfo cultureInfo)
        {
            var openingTimeFormat = GetOpeningTimeFormat(cultureInfo);

            return new ScheduleIntervalViewModel
            {
                BeginTime = string.Format(cultureInfo, openingTimeFormat, DateTime.Today.Add(openingTime.BeginingTime)),
                EndTime = string.Format(cultureInfo, openingTimeFormat, DateTime.Today.Add(openingTime.EndingTime))
            };
        }

        protected virtual StoreAddressViewModel CreateStoreAddressViewModel(CreateStoreViewModelParam param)
        {
            var addressViewModel = new StoreAddressViewModel();
            if (param.Store.FulfillmentLocation != null && param.Store.FulfillmentLocation.Addresses != null)
            {
                var overtureAddress = param.Store.FulfillmentLocation.Addresses.FirstOrDefault();
                if (overtureAddress != null)
                {
                    addressViewModel = ViewModelMapper.MapTo<StoreAddressViewModel>(overtureAddress, param.CultureInfo);

                    addressViewModel.PhoneNumber = GetFormattedAddressPhoneNumber(overtureAddress.PhoneNumber,
                        param.CultureInfo);

                    if (!string.IsNullOrWhiteSpace(overtureAddress.CountryCode))
                    {
                        var countryName = CountryService.RetrieveCountryDisplayNameAsync(new RetrieveCountryParam
                        {
                            CultureInfo = param.CultureInfo,
                            IsoCode = overtureAddress.CountryCode
                        }).Result;

                        var regionName =
                            CountryService.RetrieveRegionDisplayNameAsync(new RetrieveRegionDisplayNameParam
                            {
                                CultureInfo = param.CultureInfo,
                                IsoCode = overtureAddress.CountryCode,
                                RegionCode = overtureAddress.RegionCode
                            }).Result;

                        addressViewModel.CountryName = !string.IsNullOrWhiteSpace(countryName)
                            ? countryName
                            : overtureAddress.CountryCode.ToUpper();

                        addressViewModel.RegionName = !string.IsNullOrWhiteSpace(regionName)
                            ? regionName
                            : overtureAddress.RegionCode;
                    }
                }
            }
            return addressViewModel;
        }


        protected virtual string GetStoreLocalizedDisplayName(StoreViewModel model, CultureInfo cultureInfo)
        {
            return model.DisplayName != null && model.DisplayName.ContainsKey(cultureInfo.Name)
                ? model.DisplayName[cultureInfo.Name]
                : model.Name;
        }

        protected virtual string GetFormattedAddressPhoneNumber(string phoneNumber, CultureInfo cultureInfo)
        {
            var localFormattingString = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Store",
                Key = "PhoneNumberFormat",
                CultureInfo = cultureInfo
            });

            return localFormattingString != null && long.TryParse(phoneNumber, out long phoneNumberAsInt)
                ? string.Format(cultureInfo, localFormattingString, phoneNumberAsInt)
                : phoneNumber;
        }

        #region Google Maps Properties
        protected virtual string GetGoogleDirectionsLink(StoreAddressViewModel addressViewModel)
        {
            if (addressViewModel == null) return null;

            string[] toPoint =
            {
                addressViewModel.Line1, addressViewModel.City, addressViewModel.CountryName, addressViewModel.PostalCode
            };

            return GoogleMapsUrlProvider.GetDirectionWithEmptyStartPointLink(toPoint);
        }
        protected virtual string GetGoogleStaticMapUrl(StoreAddressViewModel addressViewModel)
        {
            return addressViewModel == null 
                ? null 
                : GoogleMaps.GetStaticMapImgUrl(GoogleMapAddressParams(addressViewModel), "roadmap");
        }

        private string[] GoogleMapAddressParams(StoreAddressViewModel model)
        {
            return model.Latitude != null && model.Longitude != null
                ? (new[]
                {
                    ((double) model.Latitude).ToString("0.00000", CultureInfo.InvariantCulture),
                    ((double) model.Longitude).ToString("0.00000", CultureInfo.InvariantCulture)
                })
                : (new[] {model.Line1, model.City, model.PostalCode, model.CountryName});
        }

        #endregion

        protected bool IsTimeInIntervals(TimeSpan time, IEnumerable<ScheduleInterval> intervals)
        {
            return intervals.Select(interval => time >= interval.BeginingTime && time < interval.EndingTime).FirstOrDefault();
        }
        protected int GetTotalPages(int total, int pageSize)
        {
            return (int)Math.Ceiling((double)total / pageSize);
        }
    }
}