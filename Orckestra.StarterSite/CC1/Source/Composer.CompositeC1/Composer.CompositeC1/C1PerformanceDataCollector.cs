using System;
using Composite.Core.Instrumentation;

namespace Orckestra.Composer.CompositeC1
{
    internal class C1PerformanceDataCollector : IPerformanceDataCollector
    {
        public IDisposable Measure(string routineDescription)
        {
            return Profiler.Measure(routineDescription);
        }
    }
}
