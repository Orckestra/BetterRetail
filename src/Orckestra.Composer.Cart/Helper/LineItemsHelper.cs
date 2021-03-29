using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Helper
{
    public static class LineItemsHelper
    {
        /// <summary>
        /// Setting free price for a gift item, avoiding reward info
        /// </summary>
        /// <param name="cart"></param>
        public static void PrepareGiftLineItems(Overture.ServiceModel.Orders.Cart cart)
        {
            if (cart?.Shipments == null || cart.Shipments.Count == 0) return;

            foreach (Shipment shipment in cart.Shipments)
            {
                if (shipment?.LineItems == null || shipment.LineItems.Count == 0) continue;
                foreach (LineItem lineitem in shipment.LineItems)
                {
                    if (lineitem == null || !lineitem.IsGiftItem) continue;

                    if (lineitem.DiscountAmount != null)
                    {
                        if (cart.DiscountTotal != null)
                        {
                            cart.DiscountTotal -= lineitem.DiscountAmount;
                        }
                        cart.LineItemLevelDiscount -= (decimal)lineitem.DiscountAmount;
                        cart.SubTotalDiscount -= (decimal)lineitem.DiscountAmount;
                    }

                    #pragma warning disable CS0618 // Type or member is obsolete
                    lineitem.ListPrice = lineitem.DefaultListPrice = lineitem.CurrentPrice =
                    lineitem.RegularPrice = lineitem.DefaultPrice = lineitem.DiscountAmount =
                    lineitem.Total = lineitem.TotalWithoutDiscount = 0;
                    #pragma warning restore CS0618 // Type or member is obsolete

                    if (lineitem?.Rewards == null || lineitem.Rewards.Count == 0) continue;

                    foreach (Reward lineitemReward in lineitem.Rewards)
                    {
                        Reward shipmentReward = shipment.Rewards.Find(x => x.Id == lineitemReward.Id);
                        shipment.Rewards.Remove(shipmentReward);
                    }
                    lineitem.Rewards = new List<Reward>();
                }
            }
        }
    }
}