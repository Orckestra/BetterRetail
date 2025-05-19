using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Parameters;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class SearchBaseController : Controller
    {
        protected ISearchRequestContext SearchRequestContext { get; private set; }

        protected SearchBaseController(ISearchRequestContext searchRequestContext)
        {
            SearchRequestContext = searchRequestContext ?? throw new ArgumentNullException(nameof(searchRequestContext));
        }

        public virtual async Task<ActionResult> PageHeader(string keywords)
        {
            var pageHeaderViewModel = await SearchRequestContext.GetPageHeaderViewModelAsync(new GetPageHeaderParam
            {
                Keywords = keywords,
                IsPageIndexed = IsPageIndexed()

            }).ConfigureAwait(false);

            return View(pageHeaderViewModel);
        }

        protected virtual bool IsPageIndexed()
        {
            return !Request.QueryString.HasKeys();
        }
    }
}
