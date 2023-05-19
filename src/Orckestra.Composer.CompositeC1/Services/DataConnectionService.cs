using Composite.Data;
using System;
using System.Globalization;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class DataConnectionService: IDataConnectionService
    {
        public IDataConnectionAdapter GetDataConnection(CultureInfo culture)
        {
            return new DataConnectionAdapter(culture);
        }
    }

    public interface IDataConnectionAdapter : IDisposable
    {
        SitemapNavigator SitemapNavigator { get; }
        PageNode GetPageNodeById(Guid id);
    }

    public class DataConnectionAdapter : IDataConnectionAdapter
    {
        private readonly DataConnection _dataConnection;
        public DataConnectionAdapter(CultureInfo culture)
        {
            _dataConnection = new DataConnection(culture);
        }

        public virtual SitemapNavigator SitemapNavigator => _dataConnection.SitemapNavigator;

        public virtual PageNode GetPageNodeById(Guid id)
        {
            return SitemapNavigator.GetPageNodeById(id);
        }

        public void Dispose()
        {
            _dataConnection.Dispose();
        }
    }
}
