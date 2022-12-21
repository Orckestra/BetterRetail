using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Data;
using Composite.Data.Types;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Tests.Mocks;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.CompositeC1.Providers.MainMenu;

namespace Orckestra.Composer.CompositeC1.Tests.Utils
{
    [TestFixture]
    public class GoogleAnalyticsNavigationUrlHelper_BuildUrl
    {
        private AutoMocker _container;
        private GoogleAnalyticsNavigationUrlProvider _sut;
        private string _customUrl = "/en-CA/women";

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _sut = _container.CreateInstance<GoogleAnalyticsNavigationUrlProvider>();
        }

        [Test]
        public void WHEN_requested_MainMenuItems_SHOULD_return_expected_result()
        {
            //Arrange
            var menuItems = new List<MainMenuItemWrapper>();
            menuItems.Add(new MainMenuItemWrapper { Url = null, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuItemWrapper { Url = string.Empty, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat2", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat1.1", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1")).Id });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat1.2", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1")).Id });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat1.1.1", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1.1")).Id });
            menuItems.Add(new MainMenuItemWrapper { Url = _customUrl, DisplayName = "cat1.1.2", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1.1")).Id });

            var menuItemsMap = menuItems.ToDictionary(_ => _.Id);

            var expectedResult = new List<string>
            {
                string.Empty,
                string.Empty,
                "/en-CA/women?origin=dropdown&c1=cat1&clickedon=cat1",
                "/en-CA/women?origin=dropdown&c1=cat2&clickedon=cat2",
                "/en-CA/women?origin=dropdown&c1=cat1&c2=cat11&clickedon=cat11",
                "/en-CA/women?origin=dropdown&c1=cat1&c2=cat12&clickedon=cat12",
                "/en-CA/women?origin=dropdown&c1=cat1&c2=cat11&c3=cat111&clickedon=cat111",
                "/en-CA/women?origin=dropdown&c1=cat1&c2=cat11&c3=cat112&clickedon=cat112"
            };

            //Act
            var result = menuItems.Select(menuItem => _sut.BuildUrl(menuItem, menuItemsMap, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Dropdown));

            //Assert
            CollectionAssert.AreEqual(expectedResult.OrderBy(foo => foo), result.OrderBy(foo => foo));
        }

        [Test]
        public void WHEN_requested_FooterItems_SHOULD_return_expected_result()
        {
            //Arrange
            var menuItems = new List<Footer>();
            menuItems.Add(new MainMenuMock { Url = null, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuMock { Url = string.Empty, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat2", Id = Guid.NewGuid(), ParentId = null });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1.1", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1")).Id });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1.2", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1")).Id });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1.1.1", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1.1")).Id });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1.1.2", Id = Guid.NewGuid(), ParentId = menuItems.First(x => x.DisplayName.Equals("cat1.1")).Id });

            var expectedResult = new List<string>
            {
                string.Empty,
                string.Empty,
                "/en-CA/women?origin=footer&c1=cat1&clickedon=cat1",
                "/en-CA/women?origin=footer&c1=cat2&clickedon=cat2",
                "/en-CA/women?origin=footer&c1=cat1&c2=cat11&clickedon=cat11",
                "/en-CA/women?origin=footer&c1=cat1&c2=cat12&clickedon=cat12",
                "/en-CA/women?origin=footer&c1=cat1&c2=cat11&c3=cat111&clickedon=cat111",
                "/en-CA/women?origin=footer&c1=cat1&c2=cat11&c3=cat112&clickedon=cat112"
            };

            //Act
            var result = menuItems.Select(menuItem => _sut.BuildUrl(menuItem, menuItems, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Footer));

            //Assert
            CollectionAssert.AreEqual(expectedResult.OrderBy(foo => foo), result.OrderBy(foo => foo));
        }

        [Test]
        public void WHEN_requested_StickyHeadertems_SHOULD_return_expected_result()
        {
            //Arrange
            var menuItems = new List<StickyHeader>();
            menuItems.Add(new MainMenuMock { Url = null, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = string.Empty, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat2", Id = Guid.NewGuid() });

            var expectedResult = new List<string>
            {
                string.Empty,
                string.Empty,
                "/en-CA/women?origin=sticky&c1=cat1&clickedon=cat1",
                "/en-CA/women?origin=sticky&c1=cat2&clickedon=cat2",
            };

            //Act
            var result = menuItems.Select(menuItem => _sut.BuildUrl(menuItem, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Sticky));

            //Assert
            CollectionAssert.AreEqual(expectedResult.OrderBy(foo => foo), result.OrderBy(foo => foo));
        }

        [Test]
        public void WHEN_requested_OptionalHeadertems_SHOULD_return_expected_result()
        {
            //Arrange
            var menuItems = new List<HeaderOptionalLink>();
            menuItems.Add(new MainMenuMock { Url = null, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = string.Empty, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat2", Id = Guid.NewGuid() });

            var expectedResult = new List<string>
            {
                string.Empty,
                string.Empty,
                "/en-CA/women?origin=dropdown&c1=cat1&clickedon=cat1",
                "/en-CA/women?origin=dropdown&c1=cat2&clickedon=cat2",
            };

            //Act
            var result = menuItems.Select(menuItem => _sut.BuildUrl(menuItem, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Dropdown));

            //Assert
            CollectionAssert.AreEqual(expectedResult.OrderBy(foo => foo), result.OrderBy(foo => foo));
        }

        [Test]
        public void WHEN_requested_Footertems_SHOULD_return_expected_result()
        {
            //Arrange
            var menuItems = new List<FooterOptionalLink>();
            menuItems.Add(new MainMenuMock { Url = null, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = string.Empty, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat1", Id = Guid.NewGuid() });
            menuItems.Add(new MainMenuMock { Url = _customUrl, DisplayName = "cat2", Id = Guid.NewGuid() });

            var expectedResult = new List<string>
            {
                string.Empty,
                string.Empty,
                "/en-CA/women?origin=footer&c1=cat1&clickedon=cat1",
                "/en-CA/women?origin=footer&c1=cat2&clickedon=cat2",
            };

            //Act
            var result = menuItems.Select(menuItem => _sut.BuildUrl(menuItem, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Footer));

            //Assert
            CollectionAssert.AreEqual(expectedResult.OrderBy(foo => foo), result.OrderBy(foo => foo));
        }
    }
}
