using Orckestra.Composer.Enums;

namespace Orckestra.Composer.Repositories
{
    public interface ILookupRepositoryFactory
    {
        ILookupRepository CreateLookupRepository(LookupType lookupType);
    }
}