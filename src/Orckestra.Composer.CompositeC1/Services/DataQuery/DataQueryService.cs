using System;
using System.Linq;
using System.Web;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Services.DataQuery
{
    internal class DataQueryService : IDataQueryService, IDisposable
    {
        private readonly HttpContext _preservedContext;
        private readonly Lazy<DataConnection> _connection;

        public DataQueryService()
        {
            _preservedContext = HttpContext.Current;

            _connection = new Lazy<DataConnection>(() =>
            {
                // DataConnection keeps its state either in HttpContext or in thread local storage, if HttpContext isn't available
                // It is important to restore the HttpContext, as
                // a) Thread local storage usage would require DataConnection.Dispose() method to be called on the same thread.
                // b) With using async rendering, the thread is going to be different at the end of the request, where DataQueryService is disposed.
                if (_preservedContext != null)
                {
                    HttpContext.Current = _preservedContext;
                }

                return new DataConnection();
            });
        }

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