﻿using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class MembershipController : MembershipBaseController
    {
        public MembershipController(
            IMyAccountUrlProvider myAccountUrlProvider, 
            IComposerContext composerContext, 
            IMembershipViewService membershipViewService) 
            : base(
            myAccountUrlProvider, 
            composerContext, 
            membershipViewService) 
        {
        }
    }
}
