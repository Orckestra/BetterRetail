using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Requests.Products.Inventory;

namespace Orckestra.Composer.Tests.Repositories
{
    [TestFixture]
    public class InventoryRepositoryFindInventoryItemStatus
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            var ovMock = Container.GetMock<IComposerOvertureClient>();
            ovMock.Setup(o => o.SendAsync(It.IsAny<FindInventoryItemStatusByLocationAndSkusRequest>()))
                .ReturnsAsync(new List<InventoryItemAvailability>());
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_RETURN_Empty_Result()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();

            var result = await inventoryRepository.FindInventoryItemStatus(new FindInventoryItemStatusParam
            {
                Scope = GetRandom.String(25),
                Date = GetRandom.DateTime(),
                Skus = new List<string>
                {
                    GetRandom.String(25)
                },
                InventoryLocationId = GetRandom.String(8)
            });

            Assert.NotNull(result);
        }

        [Test]
        public void WHEN_Passing_Null_Param_THROW_ArgumentException()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();
            var param = new FindInventoryItemStatusParam
            {
                Scope = null,
                Date = GetRandom.DateTime(),
                Skus = new List<string>
                {
                    GetRandom.String(25)
                }
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryRepository.FindInventoryItemStatus(param));
        }

        [Test]
        public async Task WHEN_Passing_MinValue_Date_RETURN_Empty_Result()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();

            var result = await inventoryRepository.FindInventoryItemStatus(new FindInventoryItemStatusParam
            {
                Scope = GetRandom.String(25),
                Date = DateTime.MinValue,
                Skus = new List<string>
                {
                    GetRandom.String(25)
                },
                InventoryLocationId = GetRandom.String(8)
            });

            Assert.NotNull(result);
        }

        [Test]
        public void WHEN_Passing_Null_ScopeId_THROW_ArgumentException()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();
            var param = new FindInventoryItemStatusParam
            {
                Scope = null,
                Date = GetRandom.DateTime(),
                Skus = new List<string>
                {
                    GetRandom.String(25)
                }
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryRepository.FindInventoryItemStatus(param));
        }

        [Test]
        public void WHEN_Passing_Null_Skus_THROW_ArgumentException()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();
            var param = new FindInventoryItemStatusParam
            {
                Scope = GetRandom.String(25),
                Date = GetRandom.DateTime(),
                Skus = null
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryRepository.FindInventoryItemStatus(param));
        }

        [Test]
        public void WHEN_Passing_Empty_Skus_THROW_ArgumentException()
        {
            var inventoryRepository = Container.CreateInstance<InventoryRepository>();
            var param = new FindInventoryItemStatusParam
            {
                Scope = GetRandom.String(25),
                Date = GetRandom.DateTime(),
                Skus = new List<string>()
            };

            Assert.ThrowsAsync<ArgumentException>(() => inventoryRepository.FindInventoryItemStatus(param));
        }
    }
}
