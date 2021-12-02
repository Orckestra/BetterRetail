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
using Orckestra.Composer.Search.Providers;

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
        public void WHEN_search_result_view_model_is_set_SHOULD_not_call_search_service()
        {
            //Arrange
            var container = new AutoMocker();
            var productRequestContext = container.CreateInstance<SearchRequestContext>();
      
            _cultureInfo = new CultureInfo("en-US");
            _scope = GetRandom.String(10);

              
            _viewModel = new SearchViewModel();
            {    };

            container.GetMock<HttpRequestBase>().Setup(m => m.Url).Returns(new Uri(@"https://contoso.com/search"));
            container.GetMock<HttpRequestBase>().Setup(m => m.ApplicationPath).Returns("C:/website");
            container.GetMock<IComposerContext>().Setup(m => m.CultureInfo).Returns(_cultureInfo);
            container.GetMock<IComposerContext>().Setup(m => m.Scope).Returns(_scope);
            container.GetMock<ISearchViewService>().Setup(m => m.GetSearchViewModelAsync(It.IsAny<SearchCriteria>())).ReturnsAsync(_viewModel);
            container.GetMock<IBaseSearchCriteriaProvider>().Setup(m => m.GetSearchCriteriaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>())).ReturnsAsync(new SearchCriteria { });
            var mock = container.GetMock<ISearchViewService>();

            //Act
            var viewModel =  productRequestContext.ProductsSearchViewModel;
            var sameViewModel =  productRequestContext.ProductsSearchViewModel;
            sameViewModel =  productRequestContext.ProductsSearchViewModel;

            //Assert
            //service should be called only once
            mock.Verify(service => service.GetSearchViewModelAsync(It.IsAny<SearchCriteria>()), Times.Once);
            
           viewModel.Should().Be(sameViewModel);
        }
    }
}