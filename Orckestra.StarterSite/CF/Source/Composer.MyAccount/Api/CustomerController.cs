using System;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Requests;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.MyAccount.Api
{
    [Authorize]
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class CustomerController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected ICustomerViewService CustomerViewService { get; private set; }
        protected ICustomerAddressViewService CustomerAddressViewService { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }

        public CustomerController(
            IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            ICartUrlProvider cartUrlProvider)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (myAccountUrlProvider == null) { throw new ArgumentNullException("myAccountUrlProvider"); }
            if (customerViewService == null) { throw new ArgumentNullException("customerViewService"); }
            if (customerAddressViewService == null) { throw new ArgumentNullException("customerAddressViewService"); }
            if (cartUrlProvider == null) { throw new ArgumentNullException("cartUrlProvider"); }

            ComposerContext = composerContext;
            MyAccountUrlProvider = myAccountUrlProvider;
            CustomerViewService = customerViewService;
            CustomerAddressViewService = customerAddressViewService;
            CartUrlProvider = cartUrlProvider;
        }

        [HttpPost]
        [ActionName("update")]
        public virtual async Task<IHttpActionResult> UpdateAccountAsync(EditAccountRequest request)
        {
            var param = new UpdateAccountParam
            {
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                PreferredLanguage = request.PreferredLanguage,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CultureInfo = ComposerContext.CultureInfo,
            };

            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(new GetMyAccountUrlParam { CultureInfo = param.CultureInfo });
            var changePasswordUrl = MyAccountUrlProvider.GetChangePasswordUrl(new GetMyAccountUrlParam { CultureInfo = param.CultureInfo });

            var viewModel = await CustomerViewService.UpdateAccountAsync(param);

            if (viewModel == null)
            {
                return Unauthorized();
            }

            viewModel.AddressListUrl = addressListUrl;
            viewModel.ChangePasswordUrl = changePasswordUrl;

            return Ok(viewModel);
        }

        [HttpGet]
        [ActionName("addresses")]
        //TODO: Change the method name for GetAdressListAsync or something similar because it confuse people
        public virtual async Task<IHttpActionResult> GetShippingAddressAsync()
        {
            var checkoutAddressStepUrl = CartUrlProvider.GetCheckoutStepUrl(new GetCheckoutStepUrlParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                StepNumber = 1,                
            });

            var addAddressUrl = CartUrlProvider.GetCheckoutAddAddressUrl(new GetCartUrlParam { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = checkoutAddressStepUrl });
            var editAddressBaseUrl = CartUrlProvider.GetCheckoutUpdateAddressBaseUrl(new GetCartUrlParam { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = checkoutAddressStepUrl });

            var viewModel = await CustomerAddressViewService.GetAddressListViewModelAsync(new GetAddressListViewModelParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                AddAddressUrl = addAddressUrl,
                EditAddressBaseUrl = editAddressBaseUrl,
                CountryCode = ComposerContext.CountryCode
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("addresses")]
        public virtual async Task<IHttpActionResult> CreateAddressAsync(EditAddressRequest request)
        {
            var returnUrl = request.ReturnUrl;
            if (string.IsNullOrWhiteSpace(returnUrl) || !UrlFormatter.IsReturnUrlValid(RequestUtils.GetBaseUrl(Request).ToString(), returnUrl))
            {
                returnUrl = MyAccountUrlProvider.GetAddressListUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });
            }

            var viewModel = await CustomerAddressViewService.CreateAddressAsync(new CreateAddressParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                EditAddress = request,
                ReturnUrl = returnUrl
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("addresses")]
        public virtual async Task<IHttpActionResult> UpdateAddressAsync(Guid id, EditAddressRequest request)
        {
            var returnUrl = request.ReturnUrl;

            if (string.IsNullOrWhiteSpace(returnUrl) || !UrlFormatter.IsReturnUrlValid(RequestUtils.GetBaseUrl(Request).ToString(), returnUrl))
            {
                returnUrl = MyAccountUrlProvider.GetAddressListUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });
            }

            var viewModel = await CustomerAddressViewService.UpdateAddressAsync(new EditAddressParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                EditAddress = request,
                AddressId = id,
                ReturnUrl = returnUrl
            });

            if (viewModel == null)
            {
                return Unauthorized();
            }

            return Ok(viewModel);
        }

        [HttpDelete]
        [ActionName("addresses")]
        public virtual async Task<IHttpActionResult> DeleteAddressAsync(Guid id)
        {
            var viewModel = await CustomerAddressViewService.DeleteAddressAsync(new DeleteAddressParam
            {
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                AddressId = id
            });

            if (viewModel == null)
            {
                return Unauthorized();
            }

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("setdefaultaddress")]
        public virtual async Task<IHttpActionResult> SetDefaultAddressAsync(Guid id)
        {
            var viewModel = await CustomerAddressViewService.SetDefaultAddressAsync(new SetDefaultAddressParam
            {
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                AddressId = id
            });

            if (viewModel == null)
            {
                return Unauthorized();
            }

            return Ok(viewModel);
        }

        [HttpGet]
        [ActionName("getdefaultaddress")]
        public virtual async Task<IHttpActionResult> GetDefaultAddressAsync()
        {
            var viewModel = await CustomerAddressViewService.GetDefaultAddressViewModelAsync(new GetAddressesForCustomerParam
            {
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
            });

            return Ok(viewModel);
        }
    }
}
