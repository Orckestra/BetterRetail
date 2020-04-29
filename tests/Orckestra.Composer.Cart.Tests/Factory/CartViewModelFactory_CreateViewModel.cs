using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Country;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class CartViewModelFactory_CreateViewModel
    {
        private static readonly string ExpectedPostalCodeRegex = GetRandom.String(5);

        private readonly IViewModelMapper _mapper;

        public AutoMocker Container { get; set; }

        public CartViewModelFactory_CreateViewModel()
        {
            _mapper = ViewModelMapperFactory.CreateFake(typeof(CartViewModelFactory).Assembly);
        }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            Container.Use(_mapper);
            Container.Use(LocalizationProviderFactory.Create());
        }

        private void UseCountryServiceMock()
        {
            UseCountryServiceMock(ExpectedPostalCodeRegex);
        }

        private void UseCountryServiceMock(string expectedPostalCodeRegex)
        {
            var country = new Country.CountryViewModel { PostalCodeRegex = expectedPostalCodeRegex };

            var countryService = new Mock<ICountryService>(MockBehavior.Strict);

            countryService.Setup(c => c.RetrieveCountryAsync(It.IsAny<RetrieveCountryParam>()))
                .ReturnsAsync(country)
                .Verifiable();

            countryService.Setup(c => c.RetrieveRegionDisplayNameAsync(It.IsAny<RetrieveRegionDisplayNameParam>()))
                .ReturnsAsync(GetRandom.String(32))
                .Verifiable();

            Container.Use(countryService);
        }

        [Test]
        public void WHEN_null_parameters_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            UseCountryServiceMock();
            
            var sut = Container.CreateInstance<CartViewModelFactory>();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => sut.CreateCartViewModel(null));
        }

        [Test]
        public void WHEN_cart_is_null_SHOULD_return_null()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = null,
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().BeNull("param.Cart was null");
        }

        [Test]
        public void WHEN_cultureInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = null,
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                BaseUrl = GetRandom.String(32)
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.CreateCartViewModel(p));
        }

        [Test]
        public void WHEN_ProductImageInfo_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                ProductImageInfo = null,
                BaseUrl = GetRandom.String(32)
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.CreateCartViewModel(p));
        }

        [Test]
        public void WHEN_imageUrls_is_null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = null
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.CreateCartViewModel(p));
        }

        [Test]
        public void WHEN_parameters_ok_SHOULD_return_notnull_CartViewModel()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            Container.Use(ViewModelMapperFactory.CreateViewNullValues());
            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
        }

        [Test]
        public void WHEN_country_postal_code_regex_is_null_SHOULD_set_postal_code_regex_in_shipping_address()
        {
            //Arrange
            UseCountryServiceMock(null);
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            Container.Use(ViewModelMapperFactory.CreateViewNullValues());
            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.ShippingAddress.PostalCodeRegexPattern.Should().BeNull();
        }

        [Test]
        public void WHEN_country_postal_code_regex_is_null_SHOULD_set_postal_code_regex_in_billing_address()
        {
            //Arrange
            UseCountryServiceMock(null);
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            Container.Use(ViewModelMapperFactory.CreateViewNullValues());
            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Payment.BillingAddress.PostalCodeRegexPattern.Should().BeNull();
        }

        [Test]
        public void WHEN_country_postal_code_regex_is_not_null_or_empty_SHOULD_set_postal_code_regex_in_shipping_address()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            Container.Use(ViewModelMapperFactory.CreateViewNullValues());
            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.ShippingAddress.PostalCodeRegexPattern.Should().Be(ExpectedPostalCodeRegex);
        }

        [Test]
        public void WHEN_country_postal_code_regex_is_not_null_or_empty_SHOULD_set_postal_code_regex_in_billing_address()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            Container.Use(ViewModelMapperFactory.CreateViewNullValues());
            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Payment.BillingAddress.PostalCodeRegexPattern.Should().Be(ExpectedPostalCodeRegex);
        }

        [Test]
        public void WHEN_cart_has_no_taxes_SHOULD_return_vm_with_empty_taxes()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = FakeCartFactory.CreateEmptyCartWithEmptyShipment(),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.Taxes.Should().BeEmpty();
        }

        [Test]
        public void WHEN_cart_has_summary_info_SHOULD_map_vm()
        {
            //Arrange
            UseCountryServiceMock();
            
            var localizationProvider = new Mock<ILocalizationProvider>();
            localizationProvider
                .Setup(l => l.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns(GetRandom.String(32))
                .Verifiable();

            Container.Use(localizationProvider);

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart()
                {
                    TaxTotal = (decimal)GetRandom.Double(0, 1000),
                    SubTotal = (decimal)GetRandom.Double(0, 1000),
                    Total = (decimal)GetRandom.Double(0, 10000),
                    DiscountTotal = (decimal)GetRandom.Double(0, 100),
                    Shipments = new List<Shipment>()
                    {
                        new Shipment()
                        {
                            Amount = (decimal) GetRandom.PositiveDouble(25)
                        }
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.Should().NotBeNull();
            vm.OrderSummary.TaxTotal.Should().NotBeNullOrWhiteSpace();
            vm.OrderSummary.SubTotal.Should().NotBeNullOrWhiteSpace();
            vm.OrderSummary.Total.Should().NotBeNullOrWhiteSpace();
            vm.OrderSummary.DiscountTotal.Should().NotBeNullOrWhiteSpace();
            vm.OrderSummary.Shipping.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_cart_has_valid_coupons_SHOULD_map_coupons()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart()
                {
                    Coupons = new List<Coupon>()
                    {
                        FakeCartFactory.CreateValidCoupon(),
                        FakeCartFactory.CreateValidCoupon(),
                        FakeCartFactory.CreateValidCoupon()
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                IncludeInvalidCouponsMessages = true
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Coupons.Should().NotBeNull();
            vm.Coupons.ApplicableCoupons.Should().NotBeNullOrEmpty();
            vm.Coupons.ApplicableCoupons.Should().HaveSameCount(p.Cart.Coupons);
        }

        [Test]
        public void WHEN_cart_has_invalid_coupons_and_IncludeInvalidCouponsMessages_is_false_SHOULD_not_return_messages()
        {
            //Arrange
            UseCountryServiceMock();
            
            const int validCouponsCount = 1;

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart()
                {
                    Coupons = new List<Coupon>()
                    {
                        FakeCartFactory.CreateInvalidCoupon(),
                        FakeCartFactory.CreateInvalidCoupon(),
                        FakeCartFactory.CreateValidCoupon()
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                IncludeInvalidCouponsMessages = false
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Coupons.Should().NotBeNull();
            vm.Coupons.ApplicableCoupons.Should().HaveCount(validCouponsCount);
            vm.Coupons.Messages.Should().BeEmpty();
        }

        [Test]
        public void WHEN_cart_has_invalid_coupons_and_IncludeInvalidCouponsMessages_is_true_SHOULD_include_message()
        {
            //Arrange
            UseCountryServiceMock();
            
            const int validCouponsCount = 1;
            const int invalidCouponsCount = 2;

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart()
                {
                    Coupons = new List<Coupon>()
                    {
                        FakeCartFactory.CreateInvalidCoupon(),
                        FakeCartFactory.CreateInvalidCoupon(),
                        FakeCartFactory.CreateValidCoupon()
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                IncludeInvalidCouponsMessages = true
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Coupons.Should().NotBeNull();
            vm.Coupons.ApplicableCoupons.Should().HaveCount(validCouponsCount);
            vm.Coupons.Messages.Should().HaveCount(invalidCouponsCount);
        }

        [Test]
        public void WHEN_cart_has_valid_parameters_SHOULD_return_shipmentId()
        {
            //Arrange
            UseCountryServiceMock();
            
            var shipmentId = Guid.NewGuid();

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            Id = shipmentId
                        }
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.CurrentShipmentId.ShouldBeEquivalentTo(shipmentId);
        }

        [Test]
        public void WHEN_cart_has_shippingMethod_SHOULD_return_shippingMethod_with_calculated_delivery_days()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            FulfillmentMethod = new FulfillmentMethod
                            {
                                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3)
                            }
                        }
                    }
                },
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.ShippingMethod.ExpectedDaysBeforeDelivery.ShouldBeEquivalentTo(3);
        }

        [Test]
        public void WHEN_cart_has_coupons_SHOULD_return_coupon_viewmodel_with_correct_values()
        {
            //Arrange
            UseCountryServiceMock();
            var rewardList = Enumerable.Repeat(new Reward
            {
                CampaignName = GetRandom.String(10),
                Amount = GetRandom.Decimal(),
                PromotionId = GetRandom.Guid(),
            }, 2).ToList();

            var p = new CreateCartViewModelParam()
            {
                Cart = FakeCartFactory.CreateCarttWithCoupon(new List<Reward>() {rewardList[0]}, new List<Reward>() {rewardList[1]}),
                CultureInfo = CultureInfo.InvariantCulture,
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Coupons.ApplicableCoupons.Should().NotBeNull();
            vm.Coupons.ApplicableCoupons.Count.Should().Be(2);

            for (int i = 0; i < vm.Coupons.ApplicableCoupons.Count; i++)
            {
                var currentReward = rewardList[i];
                var applicableCoupon = vm.Coupons.ApplicableCoupons.Find(c => c.PromotionId == currentReward.PromotionId);
                applicableCoupon.Should().NotBeNull();
                applicableCoupon.PromotionId.Should().Be(currentReward.PromotionId);
                applicableCoupon.Amount.Should().Be(currentReward.Amount);
            }
        }

        [Test]
        public void WHEN_cart_has_Payment_SHOULD_return_payment_ViewModel()
        {
            // Arrange
            UseCountryServiceMock();
            
            var cultureName = "en-US";
            var methodId = Guid.NewGuid();
            var displayName = GetRandom.String(10);
            var paymentProviderName = GetRandom.String(32);

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            
                        }
                    },
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            PaymentMethod = new PaymentMethod
                            {
                                Id = methodId,
                                PaymentProviderName = paymentProviderName,
                                DisplayName = new LocalizedString()
                                {
                                    { cultureName, displayName}
                                },
                                Type = PaymentMethodType.Cash
                            },
                            BillingAddress = new Address
                            {
                                City = "Paris"
                            }
                        }
                    }
                },
                CultureInfo = new CultureInfo(cultureName),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                },
                PaymentMethodDisplayNames = new Dictionary<string, string> { { "Cash", "TestDisplayName" } }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Payment.Should().NotBeNull();
            vm.Payment.PaymentMethod.Should().NotBeNull();
            vm.Payment.PaymentMethod.Id.ShouldBeEquivalentTo(methodId);
            vm.Payment.PaymentMethod.DisplayName.ShouldBeEquivalentTo("TestDisplayName");
            vm.Payment.PaymentMethod.PaymentProviderName.ShouldBeEquivalentTo(paymentProviderName);

            vm.Payment.BillingAddress.Should().NotBeNull();
            vm.Payment.BillingAddress.City.ShouldBeEquivalentTo("Paris");
            vm.Payment.BillingAddress.UseShippingAddress.Should().BeFalse();
        }

        [Test]
        public void WHEN_cart_has_Payment_with_use_ShippingAddress_for_billing_address_SHOULD_return_right_ViewModel()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            Address = new Address
                            {
                                City = "Paris"
                            }
                        }
                    },
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            BillingAddress = new Address
                            {
                                City = "Paris"
                            }
                        }
                    }
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.Payment.Should().NotBeNull();

            vm.Payment.BillingAddress.Should().NotBeNull();
            vm.Payment.BillingAddress.City.ShouldBeEquivalentTo("Paris");
            vm.Payment.BillingAddress.UseShippingAddress.Should().BeTrue();
        }

        [Test]
        public void WHEN_Cart_has_Additional_Fees_SHOULD_return_additional_fees()
        {
            //Arrange
            UseCountryServiceMock();
            
            var randomAdditionalFee = GetRandom.PositiveDecimal();

            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    AdditionalFeeTotal = randomAdditionalFee
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.AdditionalFeeTotal.Should().NotBeNull();
            vm.OrderSummary.AdditionalFeeTotal.ShouldBeEquivalentTo(randomAdditionalFee.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void WHEN_Cart_has_null_Additional_Fees_SHOULD_return_null()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    AdditionalFeeTotal = null
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.AdditionalFeeTotal.Should().BeNull();
        }

        [Test]
        public void WHEN_Cart_has_no_lineitem_SHOULD_return_LastCheckoutStep_0()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.CheckoutRedirectAction.LastCheckoutStep = 0;
        }

        [Test]
        public void WHEN_Cart_has_lineitem_and_lastCheckoutStep_in_bag_is_0_SHOULD_return_LastCheckoutStep_1()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    PropertyBag = new PropertyBag(new Dictionary<string, object>
                    {
                        { CartConfiguration.CartPropertyBagLastCheckoutStep, 0 }
                    }),
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            LineItems = new List<LineItem>
                            {
                                new LineItem()
                            }
                        }
                    }
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.CheckoutRedirectAction.LastCheckoutStep = 1;
        }

        [Test]
        public void WHEN_Cart_has_lineitem_and_lastCheckoutStep_in_bag_is_2_SHOULD_return_LastCheckoutStep_2()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam()
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    PropertyBag = new PropertyBag(new Dictionary<string, object>
                    {
                        { CartConfiguration.CartPropertyBagLastCheckoutStep, 2 }
                    }),
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            LineItems = new List<LineItem>
                            {
                                new LineItem()
                            }
                        }
                    }
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Should().NotBeNull();
            vm.OrderSummary.CheckoutRedirectAction.LastCheckoutStep = 2;
        }

        [TestCaseSource(typeof(PaymentsSource), "InvalidPayments")]
        public void WHEN_invalid_payments_SHOULD_use_shipping_address_as_billing_address(List<Payment> invalidPayments)
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            
                        }
                    },
                    Payments = invalidPayments
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Payment.BillingAddress.UseShippingAddress.Should().BeTrue();
        }

        private class PaymentsSource
        {
            public static IEnumerable InvalidPayments
            {
                get
                {
                    yield return new TestCaseData(null).SetName("NullPayment");
                    yield return new TestCaseData(new List<Payment>()).SetName("EmptyPayment");
                    yield return new TestCaseData(new List<Payment> { new Payment { PaymentStatus = PaymentStatus.Voided } }).SetName("VoidedPayment");
                    yield return new TestCaseData(new List<Payment> { new Payment { BillingAddress = null } }).SetName("NoBillingAddress");
                }
            }
        }

        [Test]
        public void WHEN_valid_payment_with_billing_address_equal_to_shipping_address_SHOULD_use_shipping_address_as_billing_address()
        {
            //Arrange
            UseCountryServiceMock();
            
            var anyAddress = new Address { City = "Paris" };
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            Address = anyAddress
                        }
                    },
                    Payments = new List<Payment>
                    {
                        new Payment { BillingAddress = anyAddress }
                    },
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Payment.BillingAddress.UseShippingAddress.Should().BeTrue();
        }

        [Test]
        public void WHEN_valid_payment_with_billing_address_not_equal_to_shipping_address_SHOULD_do_not_use_shipping_address_as_billing_address()
        {
            //Arrange
            UseCountryServiceMock();
            
            var p = new CreateCartViewModelParam
            {
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Shipments = new List<Shipment>
                    {
                        new Shipment
                        {
                            Address = new Address { City = "Montreal" }
                        }
                    },
                    Payments = new List<Payment>
                    {
                        new Payment { BillingAddress = new Address { City = "Paris" } }
                    }
                },
                CultureInfo = new CultureInfo("en-US"),
                BaseUrl = GetRandom.String(32),
                ProductImageInfo = new ProductImageInfo()
                {
                    ImageUrls = new List<ProductMainImage>()
                }
            };

            var sut = Container.CreateInstance<CartViewModelFactory>();

            //Act
            var vm = sut.CreateCartViewModel(p);

            //Assert
            vm.Payment.BillingAddress.UseShippingAddress.Should().BeFalse();
        }
    }
}
