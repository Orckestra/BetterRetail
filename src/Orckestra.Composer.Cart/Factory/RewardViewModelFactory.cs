using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Factory
{
    public class RewardViewModelFactory : IRewardViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public RewardViewModelFactory(IViewModelMapper viewModelMapper, ILocalizationProvider localizationProvider)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        /// <summary>
        /// Creates a list of <see cref="RewardViewModel"/>.
        /// </summary>
        /// <param name="rewards">Discounts to be mapped.</param>
        /// <param name="cultureInfo">Culture Info.</param>
        /// <param name="rewardLevels">Determines the which discount levels should be extracted. If not defined, all discount levels will be taken.</param>
        /// <returns>Enumerable of <see cref="RewardViewModel"/>. If none, should return empty enumeration, never null.</returns>
        public virtual IEnumerable<RewardViewModel> CreateViewModel(
            IEnumerable<Reward> rewards,
            CultureInfo cultureInfo,
            params RewardLevel[] rewardLevels)
        {
            if (rewards == null) { yield break; }

            var eligibleRewards = ((rewardLevels == null || rewardLevels.Length == 0)
                ? rewards
                : rewards
                .Where(d => rewardLevels.Contains(d.Level)));
                
            var comparer = new RewardEqualityComparer();

            foreach (var vm in eligibleRewards.Distinct(comparer).Select(d => ViewModelMapper.MapTo<RewardViewModel>(d, cultureInfo)))
            {
                yield return vm;
            }
        }


    }
}
