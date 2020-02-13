using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;

namespace Orckestra.Composer.LoadTest.Tests
{
    public class TestController_UpdateScopeAndFulfillmentLocation
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            //HttpContext.Current = MockHttpContextFactory.Create();
        }

        [Test]
        public async Task Update_Scope_And_Inventory_Location_SHOULD_Succeed()
        {
            var controller = _container.CreateInstance<TestController>();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());

            var actionResult = controller.UpdateScopeAndInventoryLocation("toto");
            var response = await actionResult.ExecuteAsync(new CancellationToken());

            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
        }
    }
}
