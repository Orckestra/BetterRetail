using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Factory to load and exploit metadata for ViewModels.
    /// </summary>
    public interface IViewModelMapper 
    {
        /// <summary>
        /// Produces a dictionnary of key-value pairs based on the metadata of a <see cref="BaseViewModel"/>.
        /// </summary>
        /// <param name="vm">ViewModel for which to extract metadata from May not be null.</param>
        /// <returns>Dictionnary of key-value pairs based on the metadata of the given <see cref="vm"/>'s type.</returns>
        IDictionary<string, object> ToDictionary(IBaseViewModel vm);

        T MapTo<T>(object source, string culture)
            where T : IBaseViewModel, new();

        T MapTo<T>(object source, CultureInfo culture)
            where T : IBaseViewModel, new();
    }
}