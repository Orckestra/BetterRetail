using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class VaultProfileRepositoryAddProfile
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_param_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange            
            var sut = Container.CreateInstance<VaultProfileRepository>();
            AddCreditCardParam creditCardParam = null;

            //Act            
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => sut.AddCreditCardAsync(creditCardParam));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CardHolderName_is_null_should_throw_argument_exception()
        {
            // Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = null,
                CartName = GetRandom.String(10),                
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = GetRandom.String(32),
                IpAddress = "127.0.0.1"
            };

            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CartName_is_null_should_throw_argument_exception()
        {
            // Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = null,
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = GetRandom.String(32),
                IpAddress = "127.0.0.1"
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_CustomerId_is_empty_guid_should_throw_argument_exception()
        {
            //Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = GetRandom.String(10),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = GetRandom.String(32),
                IpAddress = "127.0.0.1"
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_PaymentId_is_empty_guid_should_throw_argument_exception()
        {
            //Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = GetRandom.String(10),
                CustomerId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = GetRandom.String(32),
                IpAddress = "127.0.0.1"
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_ScopeId_is_null_should_throw_argument_exception()
        {
            //Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = GetRandom.String(10),
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = null,
                VaultTokenId = GetRandom.String(32),
                IpAddress = "127.0.0.1"
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }

        [Test]
        public void WHEN_VaultTokenId_is_null_should_throw_argument_exception()
        {
            //Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = GetRandom.String(10),
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = null,
                IpAddress = "127.0.0.1"
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }
        [Test]
        public void WHEN_IPAddress_is_null_should_throw_argument_exception()
        {
            //Arrange
            var addCreditCardParam = new AddCreditCardParam()
            {
                CardHolderName = GetRandom.String(10),
                CartName = GetRandom.String(10),
                CustomerId = GetRandom.Guid(),
                PaymentId = GetRandom.Guid(),
                Scope = GetRandom.String(32),
                VaultTokenId = GetRandom.String(32),
                IpAddress = null
            };
            var sut = Container.CreateInstance<VaultProfileRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => sut.AddCreditCardAsync(addCreditCardParam));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("param");
        }
    }
}
