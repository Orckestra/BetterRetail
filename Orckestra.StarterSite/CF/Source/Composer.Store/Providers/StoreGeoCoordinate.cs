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
            if (store == null) throw new ArgumentNullException(nameof(store));
            _store = store;
        }

        public Coordinate GetCoordinate()
        {
            if (!_store.HasLocation())
                return null;
            return new Coordinate(_store.GetLatitude(), _store.GetLongitude());
        }
    }
}
