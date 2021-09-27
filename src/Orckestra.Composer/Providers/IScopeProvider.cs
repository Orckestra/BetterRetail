using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Providers
{
    public interface IScopeProvider
    {
        string DefaultScope { get; }
    }
}
