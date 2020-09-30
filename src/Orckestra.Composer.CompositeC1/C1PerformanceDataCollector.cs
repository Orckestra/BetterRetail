using System;
using System.Web;
using Composite.Core.Instrumentation;

namespace Orckestra.Composer.CompositeC1
{
    internal class C1PerformanceDataCollector : IPerformanceDataCollector
    {
        public IDisposable Measure(string routineDescription)
        {
            var context = HttpContext.Current;

            if (context == null)
            {
                HttpContext.Current = ContextPreservationHttpModule.PreservedHttpContext.Value;
            }

            return Profiler.Measure(routineDescription);
        }
    }
}