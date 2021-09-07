using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.ViewModels.Breadcrumb;
using System;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public class BreadcrumbProvider : IBreadcrumbProvider
    {
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        public BreadcrumbProvider(IBreadcrumbViewService breadcrumbViewService, IComposerContext composerContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            return true;
        }

        public BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            return BreadcrumbViewService.CreateBreadcrumbViewModel(new GetBreadcrumbParam
            {
                CurrentPageId = currentPageId.ToString(),
                CultureInfo = ComposerContext.CultureInfo
            });
        }
    }
}
