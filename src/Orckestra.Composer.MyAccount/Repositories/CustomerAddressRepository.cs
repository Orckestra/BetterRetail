﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Caching;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers.Addresses;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.MyAccount.Repositories
{
    public class CustomerAddressRepository: ICustomerAddressRepository
    {
        protected IComposerOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public CustomerAddressRepository(IComposerOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Gets a list of all Addresses bound to a Customer account
        /// </summary>
        /// <getCustomerByIdParam name="customerId">The customer's unique identifier</getCustomerByIdParam>
        /// <getCustomerByIdParam name="scopeId">The scope to which those address belong</getCustomerByIdParam>
        /// <returns>
        /// A list of all Addresses bound to a Customer account
        /// </returns>
        public virtual Task<List<Address>> GetCustomerAddressesAsync(Guid customerId, string scopeId)
        {
            if (customerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(), nameof(customerId)); }
            if (scopeId == null) { throw new ArgumentNullException(nameof(scopeId)); }

            var getCustomerAddressesRequest = new GetCustomerAddressesRequest { CustomerId = customerId, ScopeId = scopeId };

            return OvertureClient.SendAsync(getCustomerAddressesRequest);
        }

        public virtual async Task<Address> CreateAddressAsync(Guid customerId, Address address, string scope)
        {
            //TODO: We can get rid of the await by using the extension method ExecuteAndSet on CacheProvider.
            var request = new AddAddressToCustomerRequest()
            {
                //TODO: Remove this when the bug #3694 is fixed:
                //TODO: [Blocker SEG #2879] When adding a new address in CheckOut > set as default does not work
                IsPreferredBilling = address.IsPreferredBilling || address.IsPreferredShipping,
                IsPreferredShipping = address.IsPreferredShipping || address.IsPreferredShipping
            };

            request.CustomerId = customerId;
            request.Id = address.Id;
            request.AddressName = address.AddressName;
            request.City = address.City;
            request.CountryCode = address.CountryCode;
            request.RegionCode = address.RegionCode;
            request.Line1 = address.Line1;
            request.Line2 = address.Line2;
            request.FirstName = address.FirstName;
            request.LastName = address.LastName;
            request.PhoneNumber = address.PhoneNumber;
            request.PhoneExtension = address.PhoneExtension;
            request.PostalCode = address.PostalCode;
            request.Latitude = address.Latitude;
            request.Longitude = address.Longitude;
            request.Email = address.Email;
            request.Notes = address.Notes;
            request.ScopeId = scope;
            request.IsPreferredBilling = address.IsPreferredBilling;
            request.IsPreferredShipping = address.IsPreferredShipping;

            if (address.PropertyBag != null)
            {
                request.PropertyBag = new PropertyBag(address.PropertyBag);
            }

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Address);
            cacheKey.AppendKeyParts(result.Id);
            //3.8 upgrade
            //CacheProvider.Add(cacheKey, result);
            await CacheProvider.SetAsync(cacheKey, result).ConfigureAwait(false);

            return result;
        }

        public virtual async Task<Address> UpdateAddressAsync(Guid customerId, Address address)
        {
            var request = new UpdateAddressRequest()
            {
                RelatedEntityId = customerId.ToString(),
                RelatedEntityType = "Customer"
            };

            request.AddressId = address.Id;
            request.AddressName = address.AddressName;
            request.City = address.City;
            request.CountryCode = address.CountryCode;
            request.RegionCode = address.RegionCode;
            request.Line1 = address.Line1;
            request.Line2 = address.Line2;
            request.FirstName = address.FirstName;
            request.LastName = address.LastName;
            request.PhoneNumber = address.PhoneNumber;
            request.PhoneExtension = address.PhoneExtension;
            request.PostalCode = address.PostalCode;
            request.Latitude = address.Latitude;
            request.Longitude = address.Longitude;
            request.Email = address.Email;
            request.Notes = address.Notes;
            request.IsPreferredBilling = address.IsPreferredBilling;
            request.IsPreferredShipping = address.IsPreferredShipping;
            if (address.PropertyBag != null)
            {
                request.PropertyBag = new PropertyBag(address.PropertyBag);
            }

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Address);
            cacheKey.AppendKeyParts(result.Id);
            //3.8 upgrade
            //CacheProvider.Replace(cacheKey, result);
            await CacheProvider.SetAsync(cacheKey, result).ConfigureAwait(false);

            return result;
        }

        public virtual async Task DeleteAddressAsync(Guid addressId)
        {
            var request = new RemoveAddressRequest
            {
                AddressId = addressId
            };

            await OvertureClient.SendAsync(request).ConfigureAwait(false);

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Address);
            cacheKey.AppendKeyParts(addressId);
            CacheProvider.Remove(cacheKey);
        }
    }
}