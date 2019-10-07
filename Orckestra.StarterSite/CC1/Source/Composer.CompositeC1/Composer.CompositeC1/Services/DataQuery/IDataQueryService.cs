using System.Linq;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Services.DataQuery
{
    public interface IDataQueryService
    {
        IQueryable<TData> Get<TData>() where TData : class, IData;
    };
}