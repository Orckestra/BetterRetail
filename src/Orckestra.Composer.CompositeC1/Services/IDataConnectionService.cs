using Composite.Data;
using System.Globalization;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IDataConnectionService
    {
        IDataConnectionAdapter GetDataConnection(CultureInfo cultore);
    }
}
