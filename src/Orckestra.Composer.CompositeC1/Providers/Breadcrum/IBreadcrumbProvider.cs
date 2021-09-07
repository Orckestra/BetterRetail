using Orckestra.Composer.ViewModels.Breadcrumb;
using System;

namespace Orckestra.Composer.CompositeC1.Providers.Breadcrumb
{
    public interface IBreadcrumbProvider
    {
        bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId);
        BreadcrumbViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId);
    }
}