using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    [TestFixture]
    public class SurfacedErrorLineItemValidationProvider_IsLineItemValid
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_no_message_in_cart_SHOULD_return_all_valid_line_item()
        {
            //Arrange
            var cart = new ProcessedCart
            {
                Messages = GetRandom.Boolean() ? null : new List<ExecutionMessage>()
            };

            var lineItems = new List<LineItem>
            {
                new LineItem
                {
                    Id = GetRandom.Guid()
                },
                new LineItem
                {
                    Id = GetRandom.Guid()
                }
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = lineItems.All(li => sut.ValidateLineItem(cart, li));

            //Assert
            isValid.Should().BeTrue();
            lineItems.Where(li => li.PropertyBag != null && li.PropertyBag.ContainsKey(SurfacedErrorLineItemValidationProvider.IsValidKey))
                .Should().HaveSameCount(lineItems, "process should create a Property Bag with the IsInvalid key");

            lineItems.Where(li => (bool) li.PropertyBag[SurfacedErrorLineItemValidationProvider.IsValidKey])
                .Should().HaveSameCount(lineItems, "all line items should be valid.");
        }

        [Test]
        public void WHEN_lineItem_invalid_in_message_SHOULD_return_false()
        {
            //Arrange
            var invalidLineItemId = GetRandom.Guid();
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Error,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, invalidLineItemId.ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = invalidLineItemId
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeFalse();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, false));
        }

        [Test]
        public void WHEN_message_is_not_error_SHOULD_ignore_message()
        {
            //Arrange
            var invalidLineItemId = GetRandom.Guid();
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, invalidLineItemId.ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = invalidLineItemId
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }
        
        [Test]
        public void WHEN_message_has_no_entityType_SHOULD_ignore_message()
        {
            //Arrange
            var invalidLineItemId = GetRandom.Guid();
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, invalidLineItemId.ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = invalidLineItemId
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }
        
        [Test]
        public void WHEN_message_has_other_entityType_than_lineitem_SHOULD_ignore_message()
        {
            //Arrange
            var invalidLineItemId = GetRandom.Guid();
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, GetRandom.String(12)},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, invalidLineItemId.ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = invalidLineItemId
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }
        
        [Test]
        public void WHEN_message_has_no_lineItemId_SHOULD_ignore_message()
        {
            //Arrange
            var invalidLineItemId = GetRandom.Guid();
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = invalidLineItemId
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }
        
        [Test]
        public void WHEN_lineItem_is_not_marked_as_invalid_SHOULD_return_true()
        {
            //Arrange
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, GetRandom.Guid().ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = GetRandom.Guid()
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }

        [Test]
        public void WHEN_lineItem_is_not_in_stock_SHOULD_return_false()
        {
            //Arrange
            var lineItemId = GetRandom.Guid();

            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Error,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, lineItemId.ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = lineItemId,
                Status = "Invalid status"
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeFalse();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, false));
        }

        [Test]
        public void WHEN_lineItem_is_in_stock_and_not_marked_invalid_SHOULD_return_true()
        {
            //Arrange
            var cart = new ProcessedCart
            {
                Messages = new List<ExecutionMessage>
                {
                    new ExecutionMessage
                    {
                        MessageId = GetRandom.Guid().ToString(),
                        Severity = ExecutionMessageSeverity.Unspecified,
                        PropertyBag = new PropertyBag
                        {
                            {SurfacedErrorLineItemValidationProvider.EntityTypeKey, "LineItem"},
                            {SurfacedErrorLineItemValidationProvider.LineItemIdKey, GetRandom.Guid().ToString()}
                        }
                    }
                }
            };

            var lineItem = new LineItem
            {
                Id = GetRandom.Guid(),
                Status = "InStock"
            };

            var sut = Container.CreateInstance<SurfacedErrorLineItemValidationProvider>();

            //Act
            var isValid = sut.ValidateLineItem(cart, lineItem);

            //Assert
            isValid.Should().BeTrue();
            lineItem.PropertyBag.Should().NotBeNull();
            lineItem.PropertyBag.Should()
                .Contain(new KeyValuePair<string, object>(SurfacedErrorLineItemValidationProvider.IsValidKey, true));
        }
    }
}
