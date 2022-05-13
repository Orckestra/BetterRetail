using System;
using System.Reflection;
using Fasterflect;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Describes a property from an instance of <see cref="BaseViewModel" />.
    /// </summary>
    public sealed class InstancePropertyMetadata : PropertyMetadataBase, IPropertyMetadata
    {
	    public InstancePropertyMetadata(PropertyInfo propertyInfo) : base(propertyInfo)
	    {
		    if (propertyInfo == null) { throw new ArgumentNullException(nameof(propertyInfo)); }

	        if (!propertyInfo.CanRead)
	        {
                throw new ArgumentException(string.Format("PropertyInfo '{0}' must have a getter.", propertyInfo.Name),
                    nameof(propertyInfo));
	        }

		    Getter = propertyInfo.CanRead ? propertyInfo.DelegateForGetPropertyValue(Flags.InstancePublic) : null;
		    Setter = propertyInfo.CanWrite ? propertyInfo.DelegateForSetPropertyValue(Flags.InstancePublic) : null;
	    }

        /// <summary>
        /// Gets the value of the property. Null if the property cannot be read.
        /// </summary>
        /// <param name="viewModel">Instance of <see cref="BaseViewModel" /> from which to read data from.</param>
        /// <returns>Instance of the value.</returns>
        public object GetValue(BaseViewModel viewModel)
        {
            if (viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }

            var value = Getter.Invoke(viewModel);
            return value;
        }

        /// <summary>
        /// Sets the value of the property. Does nothing if the property is read-only.
        /// </summary>
        /// <param name="viewModel">Instance of <see cref="BaseViewModel" /> into which the data should be set.</param>
        /// <param name="value">Value to set in the instance's property.</param>
        public void SetValue(BaseViewModel viewModel, object value)
        {
            if (viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }

			// We need to ignore value types, otherwise the Setter.Invoke will throw a NullReferenceException
            if (Setter == null || (value == null && PropertyType.IsValueType))
            {
                return;
            }

            Setter.Invoke(viewModel, value);
        }

        private MemberGetter Getter { get; set; }
        private MemberSetter Setter { get; set; }

    }
}