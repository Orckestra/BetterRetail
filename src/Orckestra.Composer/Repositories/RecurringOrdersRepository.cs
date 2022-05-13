using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.RecurringOrders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;


namespace Orckestra.Composer.Repositories
{
    public class RecurringOrdersRepository : IRecurringOrdersRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public RecurringOrdersRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }
        public virtual Task<ListOfRecurringOrderLineItems> GetRecurringOrderTemplates(string scope, Guid customerId)
        {
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            if (customerId == null) { throw new ArgumentNullException(nameof(customerId)); }

            var getRecurringOrderLineItemsForCustomerRequest = new GetRecurringOrderLineItemsForCustomerRequest()
            {
                CustomerId = customerId,
                ScopeId = scope
            };

            return OvertureClient.SendAsync(getRecurringOrderLineItemsForCustomerRequest);
        }

        public virtual Task<RecurringOrderProgram> GetRecurringOrderProgram(string scope, string recurringOrderProgramName)
        {
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            if (recurringOrderProgramName == null) { throw new ArgumentNullException(nameof(recurringOrderProgramName)); }

            var cacheKey = BuildRecurringOrderProgramCacheKey(scope, recurringOrderProgramName);

            var request = new GetRecurringOrderProgramRequest
            {
                RecurringOrderProgramName = recurringOrderProgramName
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        public virtual async Task<ListOfRecurringOrderLineItems> UpdateRecurringOrderTemplateLineItemQuantityAsync(UpdateRecurringOrderTemplateLineItemQuantityParam param)
        {
            var lineitems = await GetRecurringOrderTemplates(param.ScopeId, param.CustomerId).ConfigureAwait(false);

            var lineitem = GetRecurringOrderLineItemFromTemplates(lineitems, param.RecurringLineItemId);

            if (lineitem != null)
            {
                lineitem.Quantity = param.Quantity;

                var request = new AddOrUpdateRecurringOrderLineItemsRequest()
                {
                    CustomerId = param.CustomerId,
                    ScopeId = param.ScopeId,
                    LineItems = lineitems.RecurringOrderLineItems,
                    MustApplyUpdatesToRecurringCart = false
                };

                return await OvertureClient.SendAsync(request);
            }

            return new ListOfRecurringOrderLineItems();
        }
        public virtual Task<HttpWebResponse> RemoveRecurringOrderTemplateLineItem(RemoveRecurringOrderTemplateLineItemParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var request = new DeleteRecurringOrderLineItemsRequest()
            {
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId,
                RecurringOrderLineItemIds = new List<Guid>() { param.LineItemId.ToGuid() }
            };

            return OvertureClient.SendAsync(request);
        }
        public virtual Task<HttpWebResponse> RemoveRecurringOrderTemplateLineItems(RemoveRecurringOrderTemplateLineItemsParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var request = new DeleteRecurringOrderLineItemsRequest()
            {
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId,
                RecurringOrderLineItemIds = param.LineItemsIds.Select(l => l.ToGuid()).ToList()
            };

            return OvertureClient.SendAsync(request);
        }

        public virtual async Task<ListOfRecurringOrderLineItems> UpdateRecurringOrderTemplateLineItemAsync(UpdateRecurringOrderTemplateLineItemParam param)
        {
            var lineitems = await GetRecurringOrderTemplates(param.ScopeId, param.CustomerId).ConfigureAwait(false);

            var lineitem = GetRecurringOrderLineItemFromTemplates(lineitems, param.LineItemId);

            if (lineitem != null)
            {
                lineitem.RecurringOrderFrequencyName = param.RecurringOrderFrequencyName;

                var nextOccurenceWithTime = lineitem.NextOccurence;
                var newDate = new DateTime(param.NextOccurence.Year, param.NextOccurence.Month, param.NextOccurence.Day,
                                        nextOccurenceWithTime.Hour, nextOccurenceWithTime.Minute, nextOccurenceWithTime.Second, DateTimeKind.Utc);
                
                lineitem.NextOccurence = newDate;
                lineitem.ShippingAddressId = param.ShippingAddressId.ToGuid();
                lineitem.BillingAddressId = param.BillingAddressId.ToGuid();
                lineitem.PaymentMethodId = param.PaymentMethodId.ToGuid();

                lineitem.ShippingProviderId = param.ShippingProviderId.ToGuid();
                lineitem.FulfillmentMethodName = param.ShippingMethodName;

                var request = new AddOrUpdateRecurringOrderLineItemsRequest()
                {
                    CustomerId = param.CustomerId,
                    MustApplyUpdatesToRecurringCart = true,
                    ScopeId = param.ScopeId,
                    LineItems = lineitems.RecurringOrderLineItems
                };

                return await OvertureClient.SendAsync(request);
            }

            return new ListOfRecurringOrderLineItems();
        }

        public virtual Task<RecurringOrderLineItem> GetRecurringOrderTemplateDetails(GetRecurringOrderTemplateDetailParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Scope == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Scope)), nameof(param)); }
            if (param.RecurringOrderLineItemId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.RecurringOrderLineItemId)), nameof(param)); }

            var getRecurringOrderLineItemsForCustomerRequest = new GetRecurringOrderLineItemRequest()
            {
                RecurringOrderLineItemId = param.RecurringOrderLineItemId,
                ScopeId = param.Scope
            };

            return OvertureClient.SendAsync(getRecurringOrderLineItemsForCustomerRequest);
        }

        protected virtual CacheKey BuildRecurringOrderProgramCacheKey(string scope, string recurringOrderProgramName)
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.RecurringOrderPrograms)
            {
                Scope = scope
            };

            key.AppendKeyParts(recurringOrderProgramName);
            return key;
        }

        protected virtual RecurringOrderLineItem GetRecurringOrderLineItemFromTemplates(ListOfRecurringOrderLineItems lineitems, string recurringLineItemIdString)
        {
            var recurringLineItemId = recurringLineItemIdString.ToGuid();

            return lineitems.RecurringOrderLineItems?.SingleOrDefault(r => r.RecurringOrderLineItemId == recurringLineItemId);
        }
    }
}
