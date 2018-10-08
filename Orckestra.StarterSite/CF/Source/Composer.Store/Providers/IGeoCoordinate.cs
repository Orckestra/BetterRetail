using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Providers
{
    public interface IGeoCoordinate
    {
        Coordinate GetCoordinate();
        string Id { get; }
    }
}