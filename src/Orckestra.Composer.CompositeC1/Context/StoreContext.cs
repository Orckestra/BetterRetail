using Orckestra.Composer.CompositeC1.Services.PreviewMode;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.Utils;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Context
{
    public class StoreContext : IStoreContext
    {
        private readonly Lazy<StoreViewModel> _viewModel;
        protected IComposerContext ComposerContext { get; }
        protected IStoreViewService StoreViewService { get; }
        protected HttpRequestBase Request { get; }
        protected Lazy<IPreviewModeService> PreviewModeService { get; }

        public StoreContext(
            IComposerContext composerContext,
            IStoreViewService service,
            HttpRequestBase request,
            Lazy<IPreviewModeService> previewModeService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            StoreViewService = service ?? throw new ArgumentNullException(nameof(service));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            PreviewModeService = previewModeService ?? throw new ArgumentNullException(nameof(previewModeService));

            _viewModel = new Lazy<StoreViewModel>(() => GetStoreViewModelAsync().Result, true);
        }

        public virtual StoreViewModel ViewModel => _viewModel.Value;


        protected virtual Task<StoreViewModel> GetStoreViewModelAsync()
        {
            string storeNumber = Request[nameof(storeNumber)];
            return GetStoreViewModelAsync(storeNumber);
        }

        protected virtual async Task<StoreViewModel> GetStoreViewModelAsync(string storeNumber)
        {
            if (string.IsNullOrWhiteSpace(storeNumber))
            {
                return ContextHelper.HandlePreviewMode(() => GetStoreViewModelAsync(PreviewModeService.Value.GetStoreNumber()).Result);
            }

            var vm = await StoreViewService.GetStoreViewModelAsync(new GetStoreByNumberParam
            {
                StoreNumber = storeNumber,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return vm;
        }

    }
}
