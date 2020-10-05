using System.Threading.Tasks;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Grocery.Providers
{
    /// <summary>
    /// An interface for moving a cart from one scope to another.
    /// </summary>
    public interface ICartMoveProvider
    {
        /// <summary>
        /// Moves a cart from one scope to another.
        /// </summary>
        /// <param name="param">Containers with params for moving</param>
        /// <returns></returns>
        Task<ProcessedCart> MoveCart(MoveCartParam param);
    }
}