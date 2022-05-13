using System;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Services
{
    public class ProductSettingsViewService : IProductSettingsViewService
    {
        protected IProductSettingsRepository ProductSettingsRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }

        public ProductSettingsViewService(
            IProductSettingsRepository productSettingsRepository,
            IViewModelMapper viewModelMapper)
        {
            ProductSettingsRepository = productSettingsRepository;
            ViewModelMapper = viewModelMapper;
        }

        /// <summary>
        /// Retrieve the Product Settings ViewModel
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual async Task<ProductSettingsViewModel> GetProductSettings(string scope, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(scope)); }
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var overtureProductSettings = await ProductSettingsRepository.GetProductSettings(scope);
            var productSettingsViewModel = ViewModelMapper.MapTo<ProductSettingsViewModel>(overtureProductSettings, cultureInfo);

            return productSettingsViewModel;
        }
    }
}
