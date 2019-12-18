using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services
{
    public class UpdateOperation
    {
        private readonly Func<Overture.ServiceModel.Orders.Cart, object, Task> _operationAction;

        public int Order { get; private set; }

        public UpdateOperation(Func<Overture.ServiceModel.Orders.Cart, object, Task> operationAction)
            :this(int.MaxValue, operationAction)
        {
            
        }

        public UpdateOperation(int order, Func<Overture.ServiceModel.Orders.Cart, object, Task> operationAction)
        {
            Order = order;
            _operationAction = operationAction;
        }

        public Task ExecuteAsync(Overture.ServiceModel.Orders.Cart cart, object param)
        {
            return _operationAction.Invoke(cart, param);
        }
    }
}
