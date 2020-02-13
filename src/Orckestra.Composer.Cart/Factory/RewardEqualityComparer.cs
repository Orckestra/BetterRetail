using System;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public class RewardEqualityComparer : IEqualityComparer<Reward>
    {
        public bool Equals(Reward x, Reward y)
        {
            var xValue = GenerateValue(x);
            var yValue = GenerateValue(y);

            return string.Equals(xValue, yValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Reward obj)
        {
            var campaignIdHash = obj.CampaignId.GetHashCode();
            var promotionIdHash = obj.PromotionId.GetHashCode();

            return campaignIdHash + 2 * promotionIdHash;
        }

        private string GenerateValue(Reward reward)
        {
            return string.Concat(reward.CampaignId, reward.PromotionId);
        }
    }
}
