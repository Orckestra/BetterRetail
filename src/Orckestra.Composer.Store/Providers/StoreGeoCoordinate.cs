using System;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Providers
{
    public sealed class StoreGeoCoordinate : IGeoCoordinate
    {
        private readonly Overture.ServiceModel.Customers.Stores.Store _store;

        public string Id => _store.Number;

        public StoreGeoCoordinate(Overture.ServiceModel.Customers.Stores.Store store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Coordinate GetCoordinate()
        {
            return !_store.HasLocation() ? null : new Coordinate(_store.GetLatitude(), _store.GetLongitude());
        }
    }
}