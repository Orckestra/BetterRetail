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
                var preservedValue = ContextPreservationHttpModule.PreservedHttpContext.Value;

                if (preservedValue == null) return EmptyDisposable.Instance;

                HttpContext.Current = preservedValue;
            }

            return Profiler.Measure(routineDescription);
        }

        private class EmptyDisposable : IDisposable
        {
            public static readonly EmptyDisposable Instance = new EmptyDisposable();

            public void Dispose()
            {
            }
        }
    }
}