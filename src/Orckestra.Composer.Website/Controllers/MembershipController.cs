using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class MembershipController : MembershipBaseController
    {
        public MembershipController(
            IMyAccountUrlProvider myAccountUrlProvider, 
            IComposerContext composerContext, 
            IMembershipViewService membershipViewService,
            ICustomerSettings customerSettings) 
            : base(
            myAccountUrlProvider, 
            composerContext, 
            membershipViewService,
            customerSettings) 
        {
        }
    }
}
