using System;
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

        public virtual ActionResult PageHeader(string keywords)
        {
            var pageHeaderViewModel = SearchRequestContext.GetPageHeaderViewModelAsync(new GetPageHeaderParam
            {
                Keywords = keywords,
                IsPageIndexed = IsPageIndexed()

            }).Result;

            return View(pageHeaderViewModel);
        }

        protected virtual bool IsPageIndexed()
        {
            return !Request.QueryString.HasKeys();
        }
    }
}
