namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Represents a conditional result.
    /// </summary>
    /// <typeparam name="T">Any reference type</typeparam>
    public class ConditionalResult<T>
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="ConditionalResult{T}"/> contains a value
        /// </summary>
        public bool HasValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the resulting value
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalResult{T}"/> class.
        /// </summary>
        /// <remark>HasValue will be set to false</remark>
        public ConditionalResult()
        {
            HasValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalResult{T}"/> class.
        /// </summary>
        /// <param name="value">The resulting value</param>
        /// <remark>HasValue will be set to true</remark>
        public ConditionalResult(T value)
        {
            Value = value;
            HasValue = true;
        }
    }
}
