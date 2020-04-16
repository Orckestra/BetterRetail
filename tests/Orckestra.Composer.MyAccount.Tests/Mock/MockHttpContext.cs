using System.Collections;
using System.IO;
using System.Web;
using Moq;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockHttpContextFactory
    {
        public static HttpContext Create()
        {
            var capabilities = new Mock<IDictionary>();
            capabilities.SetupGet(t => t[It.IsAny<string>()]).Returns("True");
            var request = new HttpRequest("c:\\dummy.txt", "http://dummy.txt", "")
            {
                Browser = new HttpBrowserCapabilities() { Capabilities = capabilities.Object }
            };
            var response = new HttpResponse(TextWriter.Null);
            return new HttpContext(request, response);
        }
    }
}
