// ReSharper disable once CheckNamespace
namespace System.Web.Mvc
{
    public static class MvcFilterRedirectUtil
    {
        public static void Redirect(this ActionExecutingContext context, string redirectUrl)
        {
            if (context.IsChildAction)
            {
                RedirectChildAction(context, redirectUrl);
            }
            else
            {
                RedirectAction(context, redirectUrl);
            }
        }

        private static void RedirectChildAction(ActionExecutingContext filterContext, string newUrl)
        {
            filterContext.HttpContext.Response.Redirect(newUrl, false);

            if (filterContext.HttpContext.ApplicationInstance != null)
            {
                filterContext.HttpContext.Response.Flush();
                filterContext.HttpContext.ApplicationInstance.CompleteRequest();
            }
            else
            {
                //Please forgive me... I found no other way... :'(
                filterContext.HttpContext.Response.End();
            }
        }

        private static void RedirectAction(ActionExecutingContext filterContext, string newUrl)
        {
            filterContext.Result = new RedirectResult(newUrl, false);
        }
    }
}
