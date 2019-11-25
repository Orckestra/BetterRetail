using System.Web;
using Moq;

namespace Orckestra.Composer.Tests.Mock
{
    public static class HttpRequestFactory
    {
        public static Mock<HttpRequestBase> CreateDefault()
        {
            Mock<HttpRequestBase> httpRequest = new Mock<HttpRequestBase>(MockBehavior.Strict);

            return httpRequest;
        }

        public static Mock<HttpRequestBase> CreateForReadingCookies()
        {
            HttpCookieCollection dummyCookies = new HttpCookieCollection();

            Mock<HttpRequestBase> httpRequest = new Mock<HttpRequestBase>(MockBehavior.Strict);

            httpRequest.SetupGet(req => req.Cookies)
                       .Returns(dummyCookies)
                       .Verifiable();

            return httpRequest;
        }
    }
}
