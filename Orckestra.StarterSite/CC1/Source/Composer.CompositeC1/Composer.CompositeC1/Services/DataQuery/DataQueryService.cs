using System;
using System.Linq;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Services.DataQuery
{
    internal class DataQueryService : IDataQueryService, IDisposable
    {
        private readonly object _syncRoot = new object();
        private volatile DataConnection _connection;

        public IQueryable<TData> Get<TData>() where TData : class, IData
        {
            return GetConnection().Get<TData>();
        }

        private DataConnection GetConnection()
        {
            if (_connection != null)
                return _connection;

            lock (_syncRoot)
            {
                if (_connection != null)
                    return _connection;

                _connection = new DataConnection();
            }

            return _connection;
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                _connection?.Dispose();
            }
        }
    };
}