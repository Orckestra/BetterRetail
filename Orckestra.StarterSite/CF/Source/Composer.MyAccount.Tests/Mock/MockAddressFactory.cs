using FizzWare.NBuilder.Generators;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockAddressFactory
    {
        /// <summary>
        /// Gets a random Address
        /// </summary>
        /// <returns></returns>
        public static Address CreateRandom()
        {
            return new Address
            {
                Id = GetRandom.Guid(),
                AddressName = GetRandom.String(32),
                City = GetRandom.String(32),
                CountryCode = GetRandom.String(2),
                Email = GetRandom.Email(),
                FirstName = GetRandom.FirstName(),
                LastName = GetRandom.LastName(),
                IsPreferredBilling = GetRandom.Boolean(),
                IsPreferredShipping = GetRandom.Boolean(),
                Latitude = GetRandom.Double(),
                Longitude = GetRandom.Double(),
                Line1 = GetRandom.String(40),
                Line2 = GetRandom.String(40),
                Notes = GetRandom.String(1029),
                PhoneExtension = GetRandom.String(3),
                PhoneNumber = GetRandom.String(12),
                PostalCode = GetRandom.String(7),
                RegionCode = GetRandom.String(4)
            };
        }
    }
}
