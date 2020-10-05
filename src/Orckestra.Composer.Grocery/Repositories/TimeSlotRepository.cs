using System;
using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        public TimeSlotRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public IOvertureClient OvertureClient { get; }
        public ICacheProvider CacheProvider { get; }

        public virtual Task<DayAvailabilityResult> CalculateScheduleAvailabilitySlotsAsync(CalculateScheduleAvailabilitySlotsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.FulfillmentLocationId == default) { throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param)); }

            var request = new CalculateScheduleAvailabilitySlotsRequest
            {
                ScopeId = param.Scope,
                FulfillmentLocationId = param.FulfillmentLocationId,
                FulfillmentType = param.FulfillmentType,
                StartDate = param.StartDate.ToUniversalTime().Date,
                EndDate = param.EndDate.ToUniversalTime().Date,
                OrderId = param.OrderId,
                ShipmentId = param.ShipmentId,
                CalculateReservationSummary = true
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<TimeSlot> GetFulfillmentLocationTimeSlotByIdAsync(GetFulfillmentLocationTimeSlotByIdParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.FulfillmentLocationId == default) { throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param)); }
            if (param.SlotId == default) { throw new ArgumentException(GetMessageOfNull(nameof(param.SlotId)), nameof(param)); }

            var cacheKey = GetCacheKeyForFulfillmentLocationTimeSlot(param);

            var result = CacheProvider.GetOrAddAsync(cacheKey, () =>
            {
                var request = new GetFulfillmentLocationTimeSlotByIdRequest
                {
                    ScopeId = param.Scope,
                    FulfillmentLocationId = param.FulfillmentLocationId,
                    FulfillmentMethodType = param.FulfillmentMethodType,
                    SlotId = param.SlotId,
                };

                return OvertureClient.SendAsync(request);
            });

            return result;
        }

        public virtual Task<TimeSlotReservation> AddFulfillmentLocationTimeSlotReservationAsync(AddFulfillmentLocationTimeSlotReservationParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.FulfillmentLocationId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param));
            if (param.SlotId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.SlotId)), nameof(param));
            if (param.ReservationDate == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ReservationDate)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (param.ExpiryTime == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ExpiryTime)), nameof(param));
            if (param.ExpiryWarningTime == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ExpiryWarningTime)), nameof(param));

            var request = new AddFulfillmentLocationTimeSlotReservationRequest
            {
                ScopeId = param.Scope,
                FulfillmentLocationId = param.FulfillmentLocationId,
                SlotId = param.SlotId,
                CartName = param.CartName,
                CartCustomerId = param.CartCustomerId,
                ReservationDate = param.ReservationDate,
                CartScopeId = param.Scope,
                ExpiryTime = param.ExpiryTime,
                ExpiryWarningTime = param.ExpiryWarningTime,
                ReservationStatus = param.ReservationStatus
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<TimeSlotReservation> GetFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.FulfillmentLocationId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param));
            if (param.SlotReservationId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.SlotReservationId)), nameof(param));

            var request = new GetFulfillmentLocationTimeSlotReservationByIdRequest
            {
                ScopeId = param.Scope,
                SlotReservationId = param.SlotReservationId,
                FulfillmentLocationId = param.FulfillmentLocationId
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual Task<HttpWebResponse> DeleteFulfillmentLocationTimeSlotReservationByIdAsync(BaseFulfillmentLocationTimeSlotReservationParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.FulfillmentLocationId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.FulfillmentLocationId)), nameof(param));
            if (param.SlotReservationId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.SlotReservationId)), nameof(param));

            var request = new DeleteFulfillmentLocationTimeSlotReservationRequest
            {
                ScopeId = param.Scope,
                SlotReservationId = param.SlotReservationId,
                FulfillmentLocationId = param.FulfillmentLocationId
            };

            return OvertureClient.SendAsync(request);
        }

        private static CacheKey GetCacheKeyForFulfillmentLocationTimeSlot(GetFulfillmentLocationTimeSlotByIdParam param)
        {
            var cacheKey = new CacheKey(GroceryConfiguration.FulfillmentTimeSlot, param.Scope);
            cacheKey.AppendKeyParts(param.SlotId, param.FulfillmentLocationId, param.FulfillmentMethodType);

            return cacheKey;
        }

        public virtual Task<ProcessedCart> ReserveTimeSlotAsync(ReserveTimeSlotParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.Scope)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param));
            if (param.SlotId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.SlotId)), nameof(param));
            if (param.CustomerId == default) throw new ArgumentException(GetMessageOfNull(nameof(param.CustomerId)), nameof(param));
            if (param.ReservationDate == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ReservationDate)), nameof(param));
            if (string.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param));
            if (param.ExpiryTime == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ExpiryTime)), nameof(param));
            if (param.ExpiryWarningTime == null) throw new ArgumentException(GetMessageOfNull(nameof(param.ExpiryWarningTime)), nameof(param));

            var request = new ReserveTimeSlotRequest
            {
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                ShipmentId = param.ShipmentId,
                TimeSlotId = param.SlotId,
                CultureName = param.CultureInfo.Name,
                ReservationDate = param.ReservationDate,
                ExpiryTime = param.ExpiryTime,
                ExpiryWarningTime = param.ExpiryWarningTime,
            };

            return OvertureClient.SendAsync(request);
        }
    }
}