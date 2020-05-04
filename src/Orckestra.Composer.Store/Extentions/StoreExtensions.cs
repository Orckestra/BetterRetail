using System;
using System.Linq;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Utils;
using Orckestra.Composer.Store.ViewModels;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Store.Extentions
{
    public static class StoreExtensions
    {
        public static bool HasLocation(this StoreViewModel store)
        {
            return store.Address.Latitude.HasValue && store.Address.Longitude.HasValue;
        }

        public static double GetLatitude(this StoreViewModel store)
        {
            if (!store.Address.Latitude.HasValue) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(store.Address.Latitude)), nameof(store)); }
            return store.Address.Latitude.Value;
        }

        public static double GetLongitude(this StoreViewModel store)
        {
            if (!store.Address.Longitude.HasValue) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(store.Address.Longitude)), nameof(store)); }
            return store.Address.Longitude.Value;
        }


        public static bool InBounds(this StoreViewModel store, Bounds bound)
        {
            return store.Address.Latitude.HasValue && store.Address.Longitude.HasValue
                   && bound.Contains(store.Address.Latitude.Value, store.Address.Longitude.Value);
        }


        public static bool HasLocation(this Overture.ServiceModel.Customers.Stores.Store store)
        {
            var address = store.FulfillmentLocation?.Addresses.FirstOrDefault();
            if (address != null)
            {
                return address.Latitude.HasValue && address.Longitude.HasValue;
            }
            return false;
        }

        public static double GetLatitude(this Overture.ServiceModel.Customers.Stores.Store store)
        {
            var address = store.FulfillmentLocation?.Addresses.FirstOrDefault() 
                ?? throw new ArgumentException(GetMessageOfNullEmpty(nameof(store.FulfillmentLocation.Addresses)), nameof(store));

            if (!address.Latitude.HasValue) { throw new InvalidOperationException(nameof(address.Latitude)); }

            return address.Latitude.Value;
        }

        public static double GetLongitude(this Overture.ServiceModel.Customers.Stores.Store store)
        {
            var address = store.FulfillmentLocation?.Addresses.FirstOrDefault()
                ?? throw new ArgumentException(GetMessageOfNullEmpty(nameof(store.FulfillmentLocation.Addresses)), nameof(store));

            if (!address.Longitude.HasValue) { throw new InvalidOperationException(nameof(address.Longitude)); }

            return address.Longitude.Value;
        }

        public static bool InBounds(this Overture.ServiceModel.Customers.Stores.Store store, Bounds bound)
        {
            var address = store.FulfillmentLocation?.Addresses.FirstOrDefault();
            return address != null
                ? address.Latitude.HasValue && address.Longitude.HasValue 
                    && bound.Contains(address.Latitude.Value, address.Longitude.Value)
                : false;
        }

        public static double CalculateDestination(this Overture.ServiceModel.Customers.Stores.Store store, Coordinate searchPoint)
        {
            return store.HasLocation()
                ? Math.Round(GeoCodeCalculator.CalcDistance(store.GetLatitude(), store.GetLongitude(),
                    searchPoint.Lat, searchPoint.Lng, EarthRadiusMeasurement.Kilometers), 2)
                : double.MaxValue;
        }
    }
}