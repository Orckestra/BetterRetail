using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class PageNotFoundController : Controller
    {
        public virtual ActionResult PageNotFoundAnalytics()
        {
            return View();
        }
    }
}