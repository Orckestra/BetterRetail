using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery
{
    public class ExtendedCookieData
    {
        private readonly ComposerCookieDto _cookieDto;

        public ExtendedCookieData(ComposerCookieDto cookieDto)
        {
            _cookieDto = cookieDto ?? throw new ArgumentNullException(nameof(cookieDto));
        }

        public ComposerCookieDto Cookie => _cookieDto;


        private bool TryGetValue(string key, out string value)
        {
            if (_cookieDto.PropertyBag == null)
            {
                value = null;
                return false;
            }

            return _cookieDto.PropertyBag.TryGetValue(key, out value);
        }

        private void SetOrRemove(string key, string value)
        {
            if (value == null)
            {
                _cookieDto.PropertyBag?.Remove(key);
                return;
            }

            if (_cookieDto.PropertyBag == null)
            {
                _cookieDto.PropertyBag = new Dictionary<string, string>();
            }

            _cookieDto.PropertyBag[key] = value;
        }

        public bool BrowseWithoutStore
        {
            get => TryGetValue(nameof(BrowseWithoutStore), out string value) && bool.Parse(value);
            set => SetOrRemove(nameof(BrowseWithoutStore), value.ToString());
        }

        public string SelectedStoreNumber
        {
            get => TryGetValue(nameof(SelectedStoreNumber), out var value) ? value : null;
            set => SetOrRemove(nameof(SelectedStoreNumber), value);
        }

        public DateTime? SelectedDay
        {
            get
            {
                if (!TryGetValue(nameof(SelectedDay), out var value))
                    return null;

                var date = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                return date >= DateTime.Today ? date : (DateTime?)null;
            }
            set => SetOrRemove(nameof(SelectedDay), value?.ToString("O"));
        }

        public FulfillmentMethodType? FulfillmentMethodType
        {
            get => TryGetValue(nameof(FulfillmentMethodType), out var value)
                ? (FulfillmentMethodType?)Enum.Parse(typeof(FulfillmentMethodType), value)
                : null;
            set => SetOrRemove(nameof(FulfillmentMethodType), value?.ToString());
        }

        public Guid TimeSlotReservationId
        {
            get => TryGetValue(nameof(TimeSlotReservationId), out var value) ? Guid.Parse(value) : default;
            set => SetOrRemove(nameof(TimeSlotReservationId), value.ToString());
        }
    }
}
