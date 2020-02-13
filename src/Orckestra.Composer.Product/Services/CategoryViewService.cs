using System;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Services
{
    public class CategoryViewService : ICategoryViewService
    {
        protected readonly IViewModelMapper _viewModelMapper;
        protected readonly ICategoryRepository _categoryRepository;

        public CategoryViewService(IViewModelMapper viewModelMapper, ICategoryRepository categoryRepository)
        {
            if (viewModelMapper == null)
            {
                throw new ArgumentNullException("viewModelMapper");
            }

            if (categoryRepository == null)
            {
                throw new ArgumentNullException("categoryRepository");
            }

            _viewModelMapper = viewModelMapper;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Gets the categories path from the provided categoryId to the root category.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        public virtual async Task<CategoryViewModel[]> GetCategoriesPathAsync(GetCategoriesPathParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var categoriesPath = await _categoryRepository.GetCategoriesPathAsync(param).ConfigureAwait(false);
            
            var categoriesPathViewModel = categoriesPath.Select(category =>
            {
                var categoryViewModelParam = new CreateCategoryViewModelParam(category, param.CultureInfo);

                return CreateCategoryViewModel(categoryViewModelParam);
            });

            return categoriesPathViewModel.ToArray();
        }

        protected virtual CategoryViewModel CreateCategoryViewModel(CreateCategoryViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.Category == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Category"), "param"); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), "param"); }

            return _viewModelMapper.MapTo<CategoryViewModel>(param.Category, param.CultureInfo);
        }
    }
}
