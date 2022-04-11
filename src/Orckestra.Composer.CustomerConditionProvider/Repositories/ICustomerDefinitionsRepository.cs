using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.CustomerConditionProvider.Repositories
{
    public interface ICustomerDefinitionsRepository
    {
        Task<EntityDefinition> GetCustomerDefinitionAsync();
    }
}