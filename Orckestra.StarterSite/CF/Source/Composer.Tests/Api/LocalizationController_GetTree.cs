using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Api;
using Orckestra.Composer.Tests.Mock;

namespace Orckestra.Composer.Tests.Api
{
    [TestFixture]
    public class LocalizationControllerGetTree
    {
        private AutoMocker _container;
        [SetUp]
        public void Setup()
        {
            _container = new AutoMocker();
            _container.Use(LocalizationProviderFactory.CreateFromTestAssets());
            _container.Use(ComposerContextFactory.Create());
        }

        /// <summary>
        /// Security fact 
        /// </summary>
        [Test]
        public void WHEN_Returning_Json_From_HttpGet_SHOULD_Not_Return_An_Array()
        {
            //Arrange
            LocalizationController controller = _container.CreateInstance<LocalizationController>();

            string jsonResponse;
            using(StringWriter output = new StringWriter())
            {
                HttpRequest request = new HttpRequest("c:\\dummy.txt", "http://dummy.txt", "");
                request.Browser = new HttpBrowserCapabilities();
                HttpResponse response = new HttpResponse(output);
                HttpContextBase context = new HttpContextWrapper(new HttpContext(request, response));
                RouteData routeData = new RouteData();
                routeData.Values["controller"] = GetRandom.String(32);
                ControllerContext  cc = new ControllerContext(context, routeData, controller);
                controller.ControllerContext = cc;

                //Act
                controller.GetTree("en-CA").ExecuteResult(cc);
                jsonResponse = output.ToString().Trim();

                output.Close();
            }

            //Assert
            jsonResponse.Should().NotStartWith("[");
        }


        /// <summary>
        /// Security fact: 
        /// </summary>
        [Test]
        [Timeout(10000)]
        public async void WHEN_SyncToAsync_SHOULD_Not_Deadlock()
        {
            //Arrange
            LocalizationController controller = _container.CreateInstance<LocalizationController>();
            HttpRequest request = new HttpRequest("c:\\dummy.txt", "http://dummy.txt", "");
            request.Browser = new HttpBrowserCapabilities();
            HttpResponse response = new HttpResponse(TextWriter.Null);
            HttpContextBase context = new HttpContextWrapper(new HttpContext(request, response));
            RouteData routeData = new RouteData();
            routeData.Values["controller"] = GetRandom.String(32);
            ControllerContext cc = new ControllerContext(context, routeData, controller);
            controller.ControllerContext = cc;

            //Act
            await Task.Delay(1);
            var result = controller.GetTree("en-CA");

            //Assert
            //Nothing to assert, we just don't want the Timeout to proc
            result.Should().NotBeNull();
        }
    }
}
