using Orckestra.Composer.Search.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public interface IBrowseCategoryRequestContext
    {
        /// <summary>
        /// CategoryBrowsingViewModel ViewModel for the current request
        /// </summary>
        CategoryBrowsingViewModel ViewModel { get; }
    }
}
