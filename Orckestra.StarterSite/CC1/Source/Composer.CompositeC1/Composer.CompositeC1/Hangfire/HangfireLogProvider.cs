using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Hangfire
{
    public class HangfireLogProvider : ILogProvider
    {
        private static HangfireLogger _logger = new HangfireLogger();

        public ILog GetLogger(string name)
        {
            return _logger;
        }
    }
}
