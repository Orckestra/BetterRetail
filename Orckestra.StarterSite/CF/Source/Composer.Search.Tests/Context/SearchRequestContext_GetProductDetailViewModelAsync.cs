using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Search.Parameters;

namespace Orckestra.Composer.Search.Tests.Context
{
    [TestFixture]
    public class SearchRequestContextGetSearchResultViewModelAsync
    {
        private CultureInfo _cultureInfo;
        private string _scope ;
        private SearchViewModel _viewModel;
        private Mock<HttpRequestBase> _requestMock;

        [SetUp]
        public void SetUp()
        {
            _requestMock = new Mock<HttpRequestBase>();
            _requestMock.Setup(q => q.Url).Returns(new Uri("https://google.com"));
            _requestMock.Setup(q => q.ApplicationPath).Returns(@"x:\");
        }

        [Test]
        public async Task WHEN_search_result_view_model_is_set_SHOULD_not_call_search_service()
        {
            //Arrange
            var container = new AutoMocker();
            var productRequestContext = container.CreateInstance<SearchRequestContext>();
            _cultureInfo = new CultureInfo("en-US");
            _scope = GetRandom.String(10);

            var getProductParam = new GetSearchViewModelParam
            {
                Keywords = GetRandom.String(10),
                Request = _requestMock.Object,
            };
           
            _viewModel = new SearchViewModel();
            {
            };
            container.GetMock<IComposerContext>().Setup(m => m.CultureInfo).Returns(_cultureInfo);
            container.GetMock<IComposerContext>().Setup(m => m.Scope).Returns(_scope);
            container.GetMock<ISearchViewService>().Setup(m => m.GetSearchViewModelAsync(It.IsAny<SearchCriteria>())).ReturnsAsync(_viewModel);
            var mock = container.GetMock<ISearchViewService>();

            // TODO
            //Act
            var viewModel = await productRequestContext.GetSearchViewModelAsync(getProductParam);
            var sameViewModel = await productRequestContext.GetSearchViewModelAsync(getProductParam);
            sameViewModel = await productRequestContext.GetSearchViewModelAsync(getProductParam);

            //Assert
            //service should be called only once
            mock.Verify(service => service.GetSearchViewModelAsync(It.IsAny<SearchCriteria>()), Times.Once);
            
            viewModel.Should().Be(sameViewModel);
        }
    }
}