using System.Web.Mvc;

namespace Orckestra.Composer.Tests.TypeExtensions.MvcUtils
{
    public class TestNotWorkingController { }
    public class FakeClass { }
    public class NotRespectingNomenclatureCont : Controller { }
    internal class InternalController : Controller { }
    public abstract class AbstractController : Controller { }

    public class EmptyController : Controller
    {
        
    }

    public class OnlyInvalidActionsController : Controller
    {
        public void NoRouteAttributeAction() { }

        [Route("privateAction")]
        private void PrivateAction() { }

        [NonAction]
        [Route("nonaction")]
        public void NonAction() { }
    }

    public class ValidController : Controller
    {
        [Route]
        public void Index() { }

        public void NoRouteAttributeAction() { }

        [Route("privateAction")]
        private void PrivateAction() { }

        [NonAction]
        [Route("nonaction")]
        public void NonAction() { }

        [Route("complexAction")]
        public void ComplexAction(string s, int i, double d) { }
    }
}
