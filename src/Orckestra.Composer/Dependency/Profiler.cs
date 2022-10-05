using System;

namespace Orckestra.Composer.Dependency
{
    /// <summary>
    /// The profiler accessing class to resolve the current instance application-wide.
    /// </summary>
    /// <remarks>This implementation is for internal use only</remarks>
    public static class Profiler
    {
        /// <summary>
        /// The current instance of an <see cref="IProfiler"/>.
        /// </summary>
        private static IProfiler _current;

        /// <summary>
        /// Gets or sets the current instance of an <see cref="IProfiler"/>.
        /// </summary>
        /// <remarks>If none was set it will automatically initialize the instance to <see cref="NullProfiler.Instance"/></remarks>
        public static IProfiler Current
        {
            get
            {
                return _current ?? NullProfiler.Instance;
            }

            set
            {
                _current = value;
            }
        }

        /// <summary>
        ///  A non executing profiler as default instance.
        /// </summary>
        private class NullProfiler : IProfiler, IDisposable
        {
            /// <summary>
            /// The single instance of <see cref="NullProfiler"/>.
            /// </summary>
            public static readonly NullProfiler Instance = new NullProfiler();

            /// <summary>
            /// Disposes of any resources.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Returns the current instance and does not perform any profiling
            /// </summary>
            /// <param name="step">
            /// The step to add to profile.
            /// </param>
            /// <returns>
            /// The <see cref="IDisposable"/>.
            /// </returns>
            public IDisposable Step(string step)
            {
                return this;
            }

            /// <inheritdoc />
            public IDisposable Step(string step, string details)
            {
                return this;
            }

            public void LogObject(string category, object value)
            {
            }


            public void LogObject(string category, Func<object> valueFactory)
            {
            }

            public void DisableObjectLog()
            {
            }

            public void EnterContext(IProfilerContext context)
            {
            }

            public IProfilerContext GetContext()
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Extensions methods for implementations of <see cref="IProfiler"/>.
    /// </summary>
    public static class ProfilerExtensions
    {
        /// <summary>
        /// Profile a specific execution step using the provided format and arguments for the entry.
        /// </summary>
        /// <param name="profiler">
        /// The instance of <see cref="IProfiler"/>.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/> instance.
        /// </returns>
        public static IDisposable Step(this IProfiler profiler, string format, params object[] args)
        {
            return profiler.Step(string.Format(format, args));
        }
    }
}
