using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public interface IRewardViewModelFactory
    {

        /// <summary>
        ///Creates a list of <see cref="RewardViewModel"/>.
        /// </summary>
        /// <param name="rewards">Discounts to be mapped.</param>
        /// <param name="cultureInfo">Culture Info.</param>
        /// <param name="rewardLevels">Determines the which discount levels should be extracted. If not defined, all discount levels will be taken.</param>
        /// <returns>Enumerable of <see cref="RewardViewModel"/>. If none, should return empty enumeration, never null.</returns>
        IEnumerable<RewardViewModel> CreateViewModel(IEnumerable<Reward> rewards, CultureInfo cultureInfo,
            params RewardLevel[] rewardLevels);
    }
}
