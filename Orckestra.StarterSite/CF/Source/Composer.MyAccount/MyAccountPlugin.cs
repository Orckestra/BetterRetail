using Orckestra.Composer.MyAccount.Factory;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;

namespace Orckestra.Composer.MyAccount
{
    public class MyAccountPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<MembershipViewService, IMembershipViewService>();

            host.Register<CustomerRepository, ICustomerRepository>();
            host.Register<CustomerAddressRepository, ICustomerAddressRepository>();

            host.Register<CustomerAddressViewService, ICustomerAddressViewService>();

            host.Register<CustomerViewService, ICustomerViewService>();

            host.Register<CustomerViewModelFactory, ICustomerViewModelFactory>();

            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(MyAccountPlugin).Assembly);

            host.RegisterApiControllers(typeof(MyAccountPlugin).Assembly);
        }
    }
}