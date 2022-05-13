using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Search.Tests.Context
{
    [TestFixture]
    public class BrowseCategoryRequestContextGetCategoryAvailableProductsAsync
    {
        private AutoMocker _container = new AutoMocker();
        private Mock<HttpRequestBase> _requestMock;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _requestMock = new Mock<HttpRequestBase>();
            _requestMock.Setup(q => q.Url).Returns(new Uri("https://google.com"));
            _requestMock.Setup(q => q.ApplicationPath).Returns(@"x:\");
        }

    }

}
