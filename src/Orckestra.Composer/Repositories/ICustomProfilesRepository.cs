using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Requests.Customers.CustomProfiles;

namespace Orckestra.Composer.Repositories
{
    public interface ICustomProfilesRepository
    {
        Task<List<CustomProfile>> GetProfileInstances(GetCustomProfilesParam param);
        Task BulkUpdateProfiles(BulkUpdateProfilesRequest request);
    }
}
