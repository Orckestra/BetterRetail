using Composite.Data.Types;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Tests.Mocks;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using Orckestra.Composer.ViewModels.Breadcrumb;
using System.Linq.Expressions;

namespace Orckestra.Composer.CompositeC1.Tests.Services.BreadcrumbViewService
{
    [TestFixture]
    public class BreadcrumbViewService_CreateBreadcrumbViewModel
    {
        public AutoMocker Container { get; set; }

        public Guid HomePageId { get; } = Guid.NewGuid();
        public string HomePageName { get; } = "Home";

        public Guid FolderPageLevel1Id { get; } = Guid.NewGuid();
        public Guid FolderPageTypeId { get;  } = Guid.Parse("9a89191f-fc93-47ee-8b9b-022611c37fa6");
        public string FolderPageLevel1Name { get; } = "Folder";

        public Guid PageLevel1Id { get; } = Guid.NewGuid();
        public string PageLevel1Name { get; } = "PageLevel1";

        public Guid PageLevel2UnderFolderId { get; } = Guid.NewGuid();
        public string PageLevel2UnderFolderName { get; } = "PageLevel2UnderFolder";

        public Guid PageLevel2Id { get; } = Guid.NewGuid();
        public string PageLevel2Name { get; } = "PageLevel2";

        private CompositeC1.Services.BreadcrumbViewService _sut;

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            var contentPageId = Guid.NewGuid();

            var dataSource = BuildPageDataSource(contentPageId);
            var pageServiceMock = new PageServiceMock(dataSource);
            Container.Use<IPageService>(pageServiceMock);

            var mockedSiteConfiguration = new Mock<ISiteConfiguration>();

           
            mockedSiteConfiguration.Setup(a => a.GetPagesConfiguration()).Returns(new PagesConfiguration(new ISiteConfigurationMetaMock()));
            Container.Use<ISiteConfiguration>(mockedSiteConfiguration);

            _sut = Container.CreateInstance<CompositeC1.Services.BreadcrumbViewService>();

        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            GetBreadcrumbParam param = null;

            //Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.CreateBreadcrumbViewModel(param));
        }

        [Test]
        public void WHEN_PageId_does_not_match_any_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var param = new GetBreadcrumbParam
            {
                CultureInfo = CultureInfo.InvariantCulture,
                CurrentPageId = Guid.NewGuid().ToString()
            };

            //Act & Assert
            Expression<Func<BreadcrumbViewModel>> expression = () => _sut.CreateBreadcrumbViewModel(param);
            var exception = Assert.Throws<InvalidOperationException>(() => expression.Compile().Invoke());

            exception.Message.Should().StartWith("Could not find any page matching this ID.");
        }

        [Test]
        public void WHEN_Creating_breadcrumb_SHOULD_stop_at_homePage()
        {
            //Arrange
            var possiblePages = new [] { FolderPageLevel1Id, PageLevel1Id};
            var pageId = possiblePages[GetRandom.Int(0, possiblePages.Length - 1)];

            //Act
            var vm = _sut.CreateBreadcrumbViewModel(new GetBreadcrumbParam
            {
                CultureInfo = CultureInfo.InvariantCulture,
                CurrentPageId = pageId.ToString()
            });

            //Assert
            vm.Items.Should().NotBeNullOrEmpty();
            vm.Items.First().DisplayName.Should().Be(HomePageName);
        }

        [Test]
        public void WHEN_creating_breadcrumb_SHOULD_have_pages_in_order()
        {
            //Arrange
            var pageId = PageLevel2Id;
            var param = new GetBreadcrumbParam
            {
                CultureInfo = CultureInfo.InvariantCulture,
                CurrentPageId = pageId.ToString()
            };

            //Act
            var vm = _sut.CreateBreadcrumbViewModel(param);

            //Assert
            vm.Items[0].DisplayName.Should().Be(HomePageName);
            vm.Items[1].DisplayName.Should().Be(PageLevel1Name);
            vm.ActivePageName.Should().Be(PageLevel2Name);
        }

        [Test]
        public void WHEN_not_generating_folder_SHOULD_all_have_url()
        {
            //Arrange
            var pageId = PageLevel2Id;
            var param = new GetBreadcrumbParam
            {
                CurrentPageId = pageId.ToString(),
                CultureInfo = CultureInfo.InvariantCulture
            };

            //Act
            var vm = _sut.CreateBreadcrumbViewModel(param);

            //Assert
            vm.ActivePageName.Should().Be(PageLevel2Name);
            vm.Items.All(i => i.Url != null).Should().BeTrue();
        }

        [Test]
        public void WHEN_generating_folder_SHOULD_not_generate_url_for_folder()
        {
            //Arrange
            var pageId = PageLevel2UnderFolderId;
            var param = new GetBreadcrumbParam
            {
                CurrentPageId = pageId.ToString(),
                CultureInfo = CultureInfo.InvariantCulture
            };

            //Act
            var vm = _sut.CreateBreadcrumbViewModel(param);

            //Assert
            vm.ActivePageName.Should().Be(PageLevel2UnderFolderName);
            var folderItem = vm.Items.First(i => i.DisplayName == FolderPageLevel1Name);
            folderItem.Url.Should().BeNull();
        }

        private IEnumerable<IPage> BuildPageDataSource(Guid contentPageId)
        {
            return new List<PageMock>
            {
                new PageMock
                {
                    Id = HomePageId,
                    MenuTitle = HomePageName,
                    Url = GetRandom.WwwUrl(),
                    PageTypeId = contentPageId,
                    ParentPageId = Guid.Empty
                },

                new PageMock
                {
                    Id = FolderPageLevel1Id,
                    MenuTitle = FolderPageLevel1Name,
                    Url = GetRandom.WwwUrl(),
                    PageTypeId = FolderPageTypeId,
                    ParentPageId = HomePageId
                },
                new PageMock
                {
                    Id = PageLevel1Id,
                    MenuTitle = PageLevel1Name,
                    Url = GetRandom.WwwUrl(),
                    PageTypeId = contentPageId,
                    ParentPageId = HomePageId
                },
                new PageMock
                {
                    Id = PageLevel2UnderFolderId,
                    MenuTitle = PageLevel2UnderFolderName,
                    Url = GetRandom.WwwUrl(),
                    PageTypeId = contentPageId,
                    ParentPageId = FolderPageLevel1Id
                },
                new PageMock
                {
                    Id = PageLevel2Id,
                    MenuTitle = PageLevel2Name,
                    Url = GetRandom.WwwUrl(),
                    PageTypeId = contentPageId,
                    ParentPageId = PageLevel1Id
                }
            };
        }
    }
}
