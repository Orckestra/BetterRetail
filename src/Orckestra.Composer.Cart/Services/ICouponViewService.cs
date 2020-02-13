using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Marketing;

namespace Orckestra.Composer.Cart.Services
{
    public interface ICouponViewService
    {
        /// <summary>
        /// Gets all invalid coupon codes.
        /// </summary>
        /// <param name="coupons"></param>
        /// <returns></returns>
        IEnumerable<string> GetInvalidCouponsCode(IEnumerable<Coupon> coupons);
            
        /// <summary>
        /// Adds a coupon to the Cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The lightweight CartViewModel</returns>
        Task<CartViewModel> AddCouponAsync(CouponParam param);

        /// <summary>
        /// Removes a coupon from the Cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The lightweight CartViewModel</returns>
        Task<CartViewModel> RemoveCouponAsync(CouponParam param);
    }
}
