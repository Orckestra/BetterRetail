using System;
using System.Linq;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Services.DataQuery
{
    internal class DataQueryService : IDataQueryService, IDisposable
    {
        private readonly Lazy<DataConnection> _connection = new Lazy<DataConnection>(() => new DataConnection());

        public IQueryable<TData> Get<TData>() where TData : class, IData
        {
            return _connection.Value.Get<TData>();
        }

        public void Dispose()
        {
            if (_connection.IsValueCreated)
                _connection.Value.Dispose();
        }
    };
}