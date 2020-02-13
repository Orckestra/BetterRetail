using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orckestra.Composer.ViewModels
{
    public interface IViewModelMetadataRegistry
    {
        /// <summary>
        /// Gets an enumeration of metadata of View models.
        /// </summary>
        /// <param name="viewModelType">Type for which to retrieve metadata for.</param>
        /// <returns>Enumeration of metadata, or an empty enumerable if type was not discovered when host ran.</returns>
        IEnumerable<IPropertyMetadata> GetViewModelMetadata(Type viewModelType);

        void LoadViewModelMetadataInAssemblyOf(Assembly assembly);

        bool HasMetadataFor(Type viewModelType);
    }
}