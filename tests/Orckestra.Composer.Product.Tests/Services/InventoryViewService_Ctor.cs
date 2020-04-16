using System;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class InventoryViewServiceCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameter_SHOULD_Succeed()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            var inventoryViewService = new InventoryViewService(inventoryRepository.Object, viewModelMapper.Object);

            Assert.IsNotNull(inventoryViewService);
        }

        [Test]
        public void WHEN_Passing_Null_InventoryRepository_SHOULD_Throw_NullArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            Assert.Throws<ArgumentNullException>(() => new InventoryViewService(null, viewModelMapper.Object));
        }

        [Test]
        public void WHEN_Passing_Null_ViewModelMapper_SHOULD_Throw_NullArgumentException()
        {
            var inventoryRepository = new Mock<IInventoryRepository>();
            var viewModelMapper = new Mock<IViewModelMapper>();

            Assert.Throws<ArgumentNullException>(() => new InventoryViewService(inventoryRepository.Object, null));
        }
    }
}
