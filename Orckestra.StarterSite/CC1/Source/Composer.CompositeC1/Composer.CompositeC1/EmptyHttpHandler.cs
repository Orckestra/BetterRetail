using System;
using System.Web;

namespace Orckestra.Composer.CompositeC1
{
    internal class EmptyHttpHandler: IHttpHandler
    {
        private EmptyHttpHandler()
        {
        }

        static EmptyHttpHandler()
        {
            Instance = new EmptyHttpHandler();
        }

        public static EmptyHttpHandler Instance { get; private set; }


        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("This code should not be reachable");
        }
    }
}
