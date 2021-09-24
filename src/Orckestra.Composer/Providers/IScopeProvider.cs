using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Providers
{
    public interface IScopeProvider
    {
        string DefaultScope { get; }
        Scope GetScopeById(string scopeId);
    }
}
