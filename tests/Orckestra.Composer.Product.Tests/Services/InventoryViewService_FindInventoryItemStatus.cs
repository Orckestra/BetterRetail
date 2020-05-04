using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.Tests.Mock;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class InventoryViewServiceFindInventoryItemStatus
    {
        [Test]
        public async Task WHEN_Passing_Empty_Parameter_SHOULD_Return_Empty_ViewModel()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            inventoryRepository.Setup(i => i.FindInventoryItemStatus(It.IsAny<FindInventoryItemStatusParam>()))
                .ReturnsAsync(new List<InventoryItemAvailability>{
                new InventoryItemAvailability
                {
                    Date = DateTime.MinValue,
                    Identifier = new InventoryItemIdentifier
                    {
                        InventoryLocationId = string.Empty,
                        Sku = string.Empty
                    },
                    Statuses = new List<InventoryItemStatus>
                    {
                        new InventoryItemStatus
                        {
                            Quantity = null,
                            Status = GetRandom.Enumeration<InventoryStatus>()
                        }
                    }
                }
                });

            var viewModelMapper = FakeViewModelMapper.CreateFake(typeof(InventoryViewService).Assembly);

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper);
            var viewModel = await inventoryViewService.FindInventoryItemStatus(new FindInventoryItemStatusParam
            {
                Scope = string.Empty,
                Date = DateTime.MinValue,
                CultureInfo = new CultureInfo("en-CA"),
                Skus = new List<string>
                {
                    string.Empty
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(viewModel);
            Assert.That(viewModel.Count == 1);
            Assert.That(viewModel[0].Date == DateTime.MinValue);
            Assert.That(viewModel[0].Identifier.InventoryLocationId == string.Empty);
            Assert.That(viewModel[0].Identifier.Sku == string.Empty);
            Assert.That(viewModel[0].Statuses.Count == 1);
            Assert.That(viewModel[0].Statuses[0].Quantity == null);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameter_SHOULD_Return_Valid_ViewModel()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            inventoryRepository.Setup(i => i.FindInventoryItemStatus(It.IsAny<FindInventoryItemStatusParam>()))
                .ReturnsAsync(new List<InventoryItemAvailability>{
                new InventoryItemAvailability
                {
                    Date = new DateTime(2015,8,17),
                    Identifier = new InventoryItemIdentifier
                    {
                        InventoryLocationId = "S001",
                        Sku = "SKU-BIDON"
                    },
                    Statuses = new List<InventoryItemStatus>
                    {
                        new InventoryItemStatus
                        {
                            Quantity = 9999,
                            Status = InventoryStatus.PreOrder
                        }
                    }
                }
                });

            var viewModelMapper = FakeViewModelMapper.CreateFake(typeof(InventoryViewService).Assembly);

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper);
            var viewModel = await inventoryViewService.FindInventoryItemStatus(new FindInventoryItemStatusParam
            {
                Scope = "Canada",
                CultureInfo = new CultureInfo("en-CA"),
                Date = new DateTime(2015, 8, 17),
                Skus = new List<string>
                {
                    "SKU-BIDOU"
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(viewModel);
            Assert.That(viewModel.Count == 1);
            Assert.That(viewModel[0].Date == new DateTime(2015, 8, 17));
            Assert.That(viewModel[0].Identifier.InventoryLocationId == "S001");
            Assert.That(viewModel[0].Identifier.Sku == "SKU-BIDON");
            Assert.That(viewModel[0].Statuses.Count == 1);
            Assert.That(viewModel[0].Statuses[0].Quantity == 9999);
            Assert.That(viewModel[0].Statuses[0].Status == InventoryStatusEnum.PreOrder);
        }

        [Test]
        public void WHEN_Passing_Null_ScopeId_SHOULD_Throw_ArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper.Object);
            var param = new FindInventoryItemStatusParam
            {
                Scope = null,
                CultureInfo = new CultureInfo("en-CA"),
                Date = new DateTime(2015, 8, 17),
                Skus = new List<string>
                {
                    "SKU-BIDOU"
                }
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryViewService.FindInventoryItemStatus(param));
        }

        [Test]
        public void WHEN_Passing_Null_Skus_SHOULD_Throw_ArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper.Object);
            var param = new FindInventoryItemStatusParam
            {
                Scope = "Canada",
                CultureInfo = new CultureInfo("en-CA"),
                Date = new DateTime(2015, 8, 17),
                Skus = null
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryViewService.FindInventoryItemStatus(param));
        }

        [Test]
        public void WHEN_Passing_Empty_Skus_SHOULD_Throw_ArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper.Object);
            var param = new FindInventoryItemStatusParam
            {
                Scope = "Canada",
                CultureInfo = new CultureInfo("en-CA"),
                Date = new DateTime(2015, 8, 17),
                Skus = new List<string>()
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryViewService.FindInventoryItemStatus(param));
        }

        [Test]
        public void WHEN_Passing_Empty_CultureInfo_SHOULD_Throw_ArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper.Object);
            var param = new FindInventoryItemStatusParam
            {
                Scope = "Canada",
                CultureInfo = null,
                Date = new DateTime(2015, 8, 17),
                Skus = new List<string>
                    {
                        "SKU-BIDOU"
                    }
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryViewService.FindInventoryItemStatus(param));
        }
    }
}
