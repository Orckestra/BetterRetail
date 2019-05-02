using System;

namespace Orckestra.Composer
{
    /// <summary>
    /// Enables collecting performance data such as response time and memory usage.
    /// </summary>
    public interface IPerformanceDataCollector
    {
        IDisposable Measure(string routineDescription);
    }
}
