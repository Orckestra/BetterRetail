using System.Web;
using Moq;

namespace Orckestra.Composer.Tests.Mock
{
    public static class HttpResponseFactory
    {
        public static Mock<HttpResponseBase> CreateDefault()
        {
            Mock<HttpResponseBase> httpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);

            httpResponse.SetupGet(res => res.Cookies)
                .Returns(new HttpCookieCollection())
                .Verifiable();

            return httpResponse;
        }

        public static Mock<HttpResponseBase> CreateForWritingCookies()
        {
            HttpCookieCollection dummyCookies = new HttpCookieCollection();

            Mock<HttpResponseBase> httpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);

            httpResponse.SetupGet(res => res.Cookies)
                       .Returns(dummyCookies)
                       .Verifiable();

            return httpResponse;
        }
    }
}
