using System.Globalization;

namespace Orckestra.Composer.ViewModels
{
    public interface IViewModelPropertyFormatter
    {
        /// <summary>
        /// Formatter to transform the value of a property on a model class
        /// to a property on a class inheriting from <see cref="BaseViewModel"/>.
        /// Should be used in conjunction with properties annotated with <see cref="FormattingAttribute"/>.
        /// </summary>
        /// <param name="value">The value of the source model property.</param>
        /// <param name="propertyMetadata"><see cref="IPropertyMetadata"/> representing the property on the ViewModel.</param>
        /// <param name="cultureInfo">The culture the formatting belongs to.
        /// This will determine which format the resouce will come from.</param>
        /// <returns></returns>
        string Format(object value, IPropertyMetadata propertyMetadata, CultureInfo cultureInfo);
    }
}
