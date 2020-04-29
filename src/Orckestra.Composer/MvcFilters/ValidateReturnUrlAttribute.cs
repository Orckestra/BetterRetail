using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.MvcFilters
{
    public sealed class ValidateReturnUrlAttribute : ActionFilterAttribute
    {
        private const string ReturnUrlKey = "ReturnUrl";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isChildActionOnly = IsChildActionOnly(filterContext);

            if (!isChildActionOnly)
            {
                var query = GetQueryString(filterContext.HttpContext.Request);

                var returnUrl = ExtractReturnUrl(query);

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    var isLocal = IsUrlLocal(filterContext, returnUrl);

                    if (!isLocal)
                    {
                        HandleInvalidReturnUrl(filterContext, query);
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsChildActionOnly(ActionExecutingContext filterContext)
        {
            var childActionAttr = filterContext.ActionDescriptor.GetCustomAttributes(typeof (ChildActionOnlyAttribute), true);

            return childActionAttr.Any();
        }

        private IDictionary<string, string> GetQueryString(HttpRequestBase request)
        {
            var dict = new Dictionary<string, string>();

            for (int i = 0; i < request.QueryString.Count; i++)
            {
                var key = request.QueryString.GetKey(i);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    var value = request.QueryString.Get(key);
                    dict.Add(key, value);
                }
            }

            return dict;
        }

        private string ExtractReturnUrl(IDictionary<string, string> query)
        {
            var key = GetReturnUrlKey(query);

            return string.IsNullOrWhiteSpace(key)
                ? null
                : query[key];
        }

        private string GetReturnUrlKey(IDictionary<string, string> query)
        {
            var key = query.Keys.FirstOrDefault(k => string.Equals(k, ReturnUrlKey, StringComparison.InvariantCultureIgnoreCase));
            return key;
        }

        private bool IsUrlLocal(ActionExecutingContext filterContext, string returnUrl)
        {
            var baseUrl = RequestUtils.GetBaseUrl(filterContext.HttpContext.Request).ToString();

            return UrlFormatter.IsReturnUrlValid(baseUrl, returnUrl);
        }

        private void HandleInvalidReturnUrl(ActionExecutingContext filterContext, IDictionary<string, string> query)
        {
            var returnUrlKey = GetReturnUrlKey(query);

            var originalUrl = filterContext.HttpContext.Request.Url.ToString();
            var newUrl = UrlFormatter.AppendQueryString(originalUrl, new NameValueCollection()
            {
                { returnUrlKey, string.Empty }
            });

            filterContext.Redirect(newUrl);
        }        
    }
}