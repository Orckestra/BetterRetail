using Orckestra.Composer.ViewModels.LanguageSwitch;
using System;

namespace Orckestra.Composer.CompositeC1.Providers.LanguageSwitch
{
    public interface ILanguageSwitchProvider
    {
        bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId);
        LanguageSwitchViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId);
    }
}