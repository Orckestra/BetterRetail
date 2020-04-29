using System;
using System.Reflection;
using Orckestra.Composer.TypeExtensions;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Describes a property coming from the bag in a <see cref="BaseViewModel" />.
    /// </summary>
    public class BagPropertyMetadata : PropertyMetadataBase, IPropertyMetadata
    {
        public BagPropertyMetadata(PropertyInfo propertyInfo) : base(propertyInfo)
	    {}

        /// <summary>
        /// Gets the value of the property. Null if the property cannot be read.
        /// </summary>
        /// <param name="viewModel">Instance of <see cref="BaseViewModel" /> from which to read data from.</param>
        /// <returns>Instance of the value.</returns>
        public object GetValue(BaseViewModel viewModel)
        {
            if (viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }

            if (!viewModel.Bag.ContainsKey(PropertyName))
            {
                return PropertyType.GetDefaultValue();
            }

            var value = viewModel.Bag[PropertyName];
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

            viewModel.Bag[PropertyName] = value;
        }
    }
}