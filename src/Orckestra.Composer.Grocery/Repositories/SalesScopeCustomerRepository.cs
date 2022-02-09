using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Customers;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeCustomerRepository : CustomerRepository
    {
        protected IScopeProvider ScopeProvider { get; set; }

        public SalesScopeCustomerRepository(IOvertureClient overtureClient,
            ICacheProvider cacheProvider,
            IScopeProvider scopeProvider) : base(overtureClient, cacheProvider)
        {
            ScopeProvider = scopeProvider;
        }

        public override Task<Customer> GetCustomerByIdAsync(GetCustomerByIdParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.GetCustomerByIdAsync(salesParam);
        }

        public override Task<Customer> GetCustomerByUsernameAsync(GetCustomerByUsernameParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.GetCustomerByUsernameAsync(salesParam);
        }

        public override Task<CustomerQueryResult> GetCustomerByEmailAsync(GetCustomerByEmailParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.GetCustomerByEmailAsync(salesParam);
        }

        public override Task<Customer> CreateUserAsync(CreateUserParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.CreateUserAsync(salesParam);
        }

        public override Task<Customer> UpdateUserAsync(UpdateUserParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.UpdateUserAsync(salesParam);
        }

        public override Task<Customer> UpdateUserPreferredStoreAsync(UpdateUserPreferredStoreParam param)
        {
            var salesParam = param.Clone();
            salesParam.ScopeId = ScopeProvider.DefaultScope;
            return base.UpdateUserPreferredStoreAsync(salesParam);
        }

        public override Task SendResetPasswordInstructionsAsync(SendResetPasswordInstructionsParam param)
        {
            var salesParam = param.Clone();
            salesParam.Scope = ScopeProvider.DefaultScope;
            return base.SendResetPasswordInstructionsAsync(salesParam);
        }

        public override Task ResetPasswordAsync(string username, string scopeId, string newPassword, string passwordAnswer)
        {
            return base.ResetPasswordAsync(username, ScopeProvider.DefaultScope, newPassword, passwordAnswer);
        }

        public override Task ChangePasswordAsync(string username, string scopeId, string oldPassword, string newPassword)
        {
            return base.ChangePasswordAsync(username, ScopeProvider.DefaultScope, oldPassword, newPassword);
        }
    }
}