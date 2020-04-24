using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class FakeCartFactory
    {
        private static readonly CouponState[] InvalidCouponStates =
        {
            CouponState.CampaignNotFound,
            CouponState.CampaignNotLive,
            CouponState.CustomerMaximumUsed,
            CouponState.Expired,
            CouponState.GlobalMaximumUsed,
            CouponState.InvalidCoupon,
            CouponState.NotYetActive,
            CouponState.Unspecified
        };

        internal static Reward CreateRandomReward(IList<Reward> rewards, RewardLevel level)
        {
            var discount = new Reward()
            {
                Amount = (decimal) GetRandom.Double(),
                CampaignId = GetRandom.Guid(),
                CampaignName = GetRandom.Phrase(20),
                Description = GetRandom.Phrase(50),
                Id = GetRandom.Guid(),
                Level = level,
                PromotionId = GetRandom.Guid(),
                PromotionName = GetRandom.String(20),
            };

            if (rewards != null)
            {
                rewards.Add(discount);
            }

            return discount;
        }

        internal static Overture.ServiceModel.Orders.Cart CreateEmptyCartWithEmptyShipment()
        {
            return new Overture.ServiceModel.Orders.Cart()
            {
                Shipments = new List<Shipment>()
                {
                    new Shipment()
                    {
                        Taxes = new List<Tax>()
                    }
                }
            };
        }

        internal static Overture.ServiceModel.Orders.Cart CreateCartWithOrderSummaryInfo()
        {
            return new Overture.ServiceModel.Orders.Cart()
            {
                SubTotal = GetRandom.PositiveDecimal(1000),
                Shipments = new List<Shipment>()
                {
                    new Shipment()
                    {
                        Amount = GetRandom.PositiveDecimal(25)
                    }
                }
            };
        }

        internal static Tax CreateTax(CultureInfo cultureInfo, double? taxTotalAmount, string taxCode, string localizedValue)
        {
            var tax = new Tax()
            {
                Code = taxCode,
                DisplayName = new LocalizedString()
                {
                    { cultureInfo.Name, localizedValue }
                },
                Id = Guid.NewGuid(),
                TaxTotal = Convert.ToDecimal(taxTotalAmount)
            };
            return tax;
        }

        internal static LineItem CreateLineItem(IList<LineItem> lineItems, IList<Reward> rewards, bool withRewards, IList<ProductMainImage> images)
        {
            var lineItem = new LineItem()
            {
                Id = GetRandom.Guid(),
                ProductId = GetRandom.String(7),
                ProductSummary = new CartProductSummary()
                {
                    DisplayName = GetRandom.String(12)
                },
                VariantId = GetRandom.String(7),
                CurrentPrice = (decimal) GetRandom.PositiveDouble(1000),
                DefaultPrice = (decimal)GetRandom.PositiveDouble(2000),
                Quantity = GetRandom.Double(1, 15),
                Total = (decimal) GetRandom.PositiveDouble(10000)
            };

            if (withRewards)
            {
                var reward = CreateRandomReward(rewards, RewardLevel.LineItem);

                lineItem.Rewards = new List<Reward>()
                {
                    reward,
                    CreateRandomReward(rewards, RewardLevel.LineItem),
                    CreateRandomReward(rewards, RewardLevel.LineItem),
                    CreateRandomReward(rewards, RewardLevel.LineItem),
                    reward
                };
            }

            if (images != null)
            {
                var img = new ProductMainImage()
                {
                    ImageUrl = GetRandom.WwwUrl(),
                    FallbackImageUrl = GetRandom.WwwUrl(),
                    ProductId = lineItem.ProductId,
                    VariantId = lineItem.VariantId
                };

                images.Add(img);
            }

            if (lineItems != null)
            {
                lineItems.Add(lineItem);
            }

            return lineItem;
        }

        internal static Coupon CreateValidCoupon()
        {
            var coupon = new Coupon()
            {
                CouponCode = GetRandom.UpperCaseString(8),
                CouponState = CouponState.Ok,
                DisplayText = GetRandom.Phrase(20),
                Id = GetRandom.Guid(),
                PromotionId = GetRandom.Guid(),
                HasBeenConsumed = GetRandom.Boolean(),
                IsActive = GetRandom.Boolean(),
                IsDeleted = GetRandom.Boolean(),
                Mode = GetRandom.Enumeration<CouponMode>()
            };

            return coupon;
        }

        internal static Coupon CreateInvalidCoupon()
        {
            var coupon = new Coupon()
            {
                CouponCode = GetRandom.UpperCaseString(8),
                CouponState = InvalidCouponStates[GetRandom.Int(0, InvalidCouponStates.Length - 1)],
                DisplayText = GetRandom.Phrase(20),
                Id = GetRandom.Guid(),
                PromotionId = GetRandom.Guid(),
                HasBeenConsumed = GetRandom.Boolean(),
                IsActive = GetRandom.Boolean(),
                IsDeleted = GetRandom.Boolean(),
                Mode = GetRandom.Enumeration<CouponMode>()
            };

            return coupon;
        }

        internal static Overture.ServiceModel.Orders.Cart CreateCarttWithCoupon(IList<Reward> lineItemRewards, IList<Reward> shipmentRewards)
        {
            var allRewards = lineItemRewards.Concat(shipmentRewards);

            return new Overture.ServiceModel.Orders.Cart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        Rewards = shipmentRewards.ToList(),
                        LineItems = new List<LineItem>
                        {
                            new LineItem
                            {
                                Rewards = lineItemRewards.ToList()
                            }
                        }
                    }
                },
                Coupons = allRewards.Select(r => new Coupon
                {
                    PromotionId = r.PromotionId,
                    CouponState = CouponState.Ok,
                    IsActive = true
                }).ToList()
            };
        }
    }
}
