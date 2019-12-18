using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    public interface ICartMergeProvider
    {
        /// <summary>
        /// Merges the lineitems of a guest customer's cart and a logged customer's cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task MergeCartAsync(CartMergeParam param);
    }
}
