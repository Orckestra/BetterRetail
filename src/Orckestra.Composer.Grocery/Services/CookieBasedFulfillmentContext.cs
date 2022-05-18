using System;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;

namespace Orckestra.Composer.Grocery.Services
{
    /// <summary>
    /// Reads fulfillment information from a cookie/
    /// </summary>
    public class CookieBasedFulfillmentContext: IFulfillmentContext
    {
        private readonly Lazy<DateTime?> _availabilityAndPriceDate;

        public CookieBasedFulfillmentContext(ICookieAccessor<ComposerCookieDto> cookieAccessor)
        {
            if (cookieAccessor == null) throw new ArgumentNullException(nameof(cookieAccessor));

            _availabilityAndPriceDate = new Lazy<DateTime?>(() =>
            {
                var cookieData = new ExtendedCookieData(cookieAccessor.Read());

                return cookieData.SelectedDay?.Add(cookieData.SelectedTime ?? TimeSpan.Zero);
            });
        }

        public DateTime? AvailabilityAndPriceDate => _availabilityAndPriceDate.Value;
    }
}
