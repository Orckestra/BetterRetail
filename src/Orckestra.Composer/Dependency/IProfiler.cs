using System;

namespace Orckestra.Composer.Dependency
{
    /// <summary>
    /// This interface defines the contract that must be implemented by all application profiler in order to track steps and execution durations.
    /// </summary>
    /// <remarks>For internal use only.</remarks>
    public interface IProfiler
    {
        /// <summary>
        /// The step to track and record.
        /// </summary>
        /// <param name="step">
        /// The step named description.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        IDisposable Step(string step);

        /// <summary>
        /// The step to track and record.
        /// </summary>
        /// <param name="step">The step named description.
        /// The name should not contain unique identifiers of data entities to avoid memory leaks when collecting stats.</param>
        /// <param name="details">The extra details related to the step, such as entity Id or amount of records passed, etc.
        /// The details will be captured when profiling, but will be ignored when collecting statistics.</param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        IDisposable Step(string step, string details);

        /// <summary>
        /// Allows logging an object. Object have to be JSON serialization friendly and immutable.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="value">
        /// The object to log.
        /// The value should be JSON-serialization friendly and expected to not change after LogObject is called.
        /// </param>
        void LogObject(string category, object value);

        /// <summary>
        /// Allows logging an object.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="valueFactory">The value factory. The function will only be evaluated conditionally.
        /// The returning object should be JSON serialization friendly and expected to not change after LogObject is called.</param>
        void LogObject(string category, Func<object> valueFactory);

        /// <summary>
        /// Disables object logging for the current node and for the descendant nodes.
        /// Can be used to exclude requests that shouldn't be logged due to security concerns.
        /// </summary>
        void DisableObjectLog();

        /// <summary>
        /// Enters the profiler context, to be used when staring separate tasks.
        /// </summary>
        /// <param name="context">The profiling context.</param>
        void EnterContext(IProfilerContext context);

        /// <summary>
        /// Gets the profiling context.
        /// </summary>
        /// <returns>The profiling context.</returns>
        IProfilerContext GetContext();
    }

    /// <summary>
    /// Represents profiling context.
    /// </summary>
    public interface IProfilerContext
    {
    }
}
