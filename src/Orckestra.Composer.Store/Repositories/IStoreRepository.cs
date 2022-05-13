using System.Threading.Tasks;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Customers.Stores;

namespace Orckestra.Composer.Store.Repositories
{
    public interface IStoreRepository
    {
        Task<FindStoresQueryResult> GetStoresAsync(GetStoresParam getStoresParam);
        Task<Overture.ServiceModel.Customers.Stores.Store> GetStoreByNumberAsync(GetStoreByNumberParam param);
        Task<Overture.ServiceModel.Customers.Stores.Store> GetStoreAsync(GetStoreParam param);
        Task<FulfillmentSchedule> GetStoreScheduleAsync(GetStoreScheduleParam param);
    }
}
