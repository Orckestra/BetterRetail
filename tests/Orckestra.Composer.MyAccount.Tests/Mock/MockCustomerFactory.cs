using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockCustomerFactory
    {
        /// <summary>
        /// Gets a random Customer
        /// </summary>
        /// <param name="status"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static Customer CreateRandom(AccountStatus? status = null, Guid? customerId = null)
        {
            return new Customer
            {
                AccountStatus = status ?? TestingExtensions.GetRandomEnum<AccountStatus>(),
                Id = customerId ?? GetRandom.Guid(),
                CellNumber = GetRandom.String(32),
                PropertyBag = new PropertyBag(new Dictionary<string, object> { { "DefaultSubstitutionOption", "NotSelected" } }),
                Addresses = new List<Address>(),
                AddressIds = new List<Guid>(),
                Created = GetRandom.DateTime(),
                Email = GetRandom.Email(),
                Username = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                PasswordQuestion = GetRandom.String(70),
                CustomerType = TestingExtensions.GetRandomEnum<CustomerType>(),
                LastModifiedBy = GetRandom.Email(),
                CreatedBy = GetRandom.Email(),
                LastModified = GetRandom.DateTime(),
                Language = TestingExtensions.GetRandomCulture().Name,
                FaxExtension = GetRandom.String(5),
                FaxNumber = GetRandom.String(32),
                IsRecurringBuyer = GetRandom.Boolean(),
                LastActivityDate = GetRandom.DateTime(),
                LastLoginDate = GetRandom.DateTime(),
                LastOrderCurrency = GetRandom.String(32),
                LastOrderDate = GetRandom.DateTime(),
                LastOrderItemsCount = GetRandom.PositiveInt(32),
                LastOrderNumber = GetRandom.String(32),
                LastOrderStatus = GetRandom.String(32),
                LastOrderTotal = GetRandom.PositiveInt(12094),
                LastPasswordChanged = GetRandom.DateTime(),
                MiddleName = GetRandom.LastName(),
                PhoneExtension = GetRandom.String(5),
                PhoneNumber = GetRandom.String(20),
                PhoneExtensionWork = GetRandom.String(5),
                PhoneNumberWork = GetRandom.String(20),
                PreferredStoreId = GetRandom.Guid()
            };
        }
    }
}
