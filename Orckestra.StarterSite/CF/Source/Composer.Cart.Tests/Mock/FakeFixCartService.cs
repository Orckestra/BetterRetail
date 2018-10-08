using System;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    public class FakeFixCartService : IFixCartService
    {
        public Task<ProcessedCart> FixCartAsync(FixCartParam param)
        {
            return Task.FromResult(param.Cart);
        }

        public Task<ProcessedCart> SetFulfillmentLocationIfRequired(FixCartParam param)
        {
            return Task.FromResult(param.Cart);
        }
        public Task<ProcessedCart> AddPaymentIfRequired(FixCartParam param)
        {
            return Task.FromResult(param.Cart);
        }
    }
}
