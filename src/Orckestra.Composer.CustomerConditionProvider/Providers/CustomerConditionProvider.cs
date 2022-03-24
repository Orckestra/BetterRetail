using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.CustomerConditionProvider.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Tools.ConditionalContent.Providers;
using Orckestra.Tools.ConditionalContent.Types.Config;

namespace Orckestra.Composer.CustomerConditionProvider.Providers
{
    public class CustomerConditionProvider : IConditionProvider
    {
        protected IComposerContext ComposerContext { get; }
        protected ICustomerRepository CustomerRepository { get; }
        protected ICustomerDefinitionsRepository CustomerDefinitionsRepository { get; }

        public CustomerConditionProvider(IComposerContext composerContext, ICustomerRepository customerRepository, ICustomerDefinitionsRepository customerDefinitionsRepository)
        {
            ComposerContext = composerContext;
            CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            CustomerDefinitionsRepository = customerDefinitionsRepository ?? throw new ArgumentNullException(nameof(customerDefinitionsRepository));
        }

        private Dictionary<string, List<string>> Fields => new Dictionary<string, List<string>>()
        {
            {
                "Base", new List<string>()
                {
                    nameof(Customer.Language),
                    nameof(Customer.CustomerType),
                    nameof(Customer.IsRecurringBuyer)
                }
            },
            {
                "Activity", new List<string>()
                {
                    nameof(Customer.LastLoginDate),
                    nameof(Customer.LastOrderDate),
                    nameof(Customer.LastOrderItemsCount),
                    nameof(Customer.LastOrderLineItemsCount)
                }
            }
        };

        public string Name => "Customer";

        public object GetData()
        {
            var customer = GetCustomer();

            return new
            {
                Base = customer,
                Activity = customer,
                State = new
                {
                    IsLoggedIn = ComposerContext.IsAuthenticated
                },
                Custom = customer?.PropertyBag
            };
        }

        private Dictionary<string, Field> GetSubFields(EntityDefinition definition, Func<AttributeDefinition, bool> predicate)
        {
            return definition.Attributes.Where(predicate)
                .Select(a => new
                {
                    a.Name,
                    Field = new Field()
                    {
                        Label = a.DisplayName?.GetLocalizedValue("en-CA"),
                        Type = GetQueryFieldType(a)
                    }
                })
                //remove unsupported types
                .Where(a => a.Field.Type != null)
                .ToDictionary(a => a.Name, a => a.Field);
        }

        public Dictionary<string, Field> GetFields()
        {

            var definition = CustomerDefinitionsRepository.GetCustomerDefinitionAsync().Result;
            var result = new Dictionary<string, Field>();

            foreach (var field in Fields)
            {
                result.Add(field.Key, new Field()
                {
                    Type = "!struct",
                    Label = field.Key,
                    SubFields = GetSubFields(definition, a => field.Value.Contains(a.Name))
                });
            }
            result.Add("State", new Field()
            {
                Type = "!struct",
                Label = "State",
                SubFields = new Dictionary<string, Field>()
                {
                    {"IsLoggedIn", new Field()
                    {
                        Label = "IsLoggedIn",
                        Type = "boolean"
                    }}
                }
            }
                );


            result.Add("Custom", new Field()
            {
                Type = "!struct",
                Label = "Custom",
                SubFields = GetSubFields(definition, a => !a.IsBuiltIn)
            }
            );

            return result;
        }

        private string GetQueryFieldType(AttributeDefinition attributeDefinition)
        {
            switch (attributeDefinition.DataType)
            {
                case AttributeDataType.Integer:
                    return "number";
                case AttributeDataType.Decimal:
                    return "number";
                case AttributeDataType.Boolean:
                    return "boolean";
                case AttributeDataType.Text:
                    return "text";
                case AttributeDataType.DateTime:
                    return "datetime";
                case AttributeDataType.Number:
                    return "number";
            }

            return null;
        }

        private Customer GetCustomer()
        {
            Customer customer = null;

            if (!ComposerContext.IsGuest)
            {
                customer = CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam
                {
                    CultureInfo = ComposerContext.CultureInfo,
                    CustomerId = ComposerContext.CustomerId,
                    Scope = ComposerContext.Scope
                }).Result;
            }
            return customer;
        }
    }
}
