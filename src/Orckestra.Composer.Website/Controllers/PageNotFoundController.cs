using System.Web.Mvc;

namespace Orckestra.Composer.Website.Controllers
{
    public class PageNotFoundController : Controller
    {
        public virtual ActionResult PageNotFoundAnalytics()
        {
            return View();
        }
    }
}