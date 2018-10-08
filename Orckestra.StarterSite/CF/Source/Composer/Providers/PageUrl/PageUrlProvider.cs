using System.Collections.Generic;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Providers.PageUrl
{
    public abstract class PageUrlProvider : IPageUrlProvider
    {
        protected IComposerContext Context { get; set; }

        //TODO: Add the ILocalisation
        protected PageUrlProvider(IComposerContext context)
        {
            Context = context;
        }

        protected virtual PageUrl GetPageUrl()
        {
            return null;
        }

        public List<PageUrl> GetAvailableLanguageUrl()
        {
            return null;
        }
    }
}
