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
using Orckestra.Composer.Logging;

namespace Orckestra.Composer.MyAccount.Api
{
    [Authorize]
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class CustomerController : ApiController
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();
        protected IComposerContext ComposerContext { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected ICustomerViewService CustomerViewService { get; private set; }
        protected ICustomerAddressViewService CustomerAddressViewService { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected IRecurringCartUrlProvider RecurringCartUrlProvider { get; private set; }

        public CustomerController(
            IComposerContext composerContext,
            IMyAccountUrlProvider myAccountUrlProvider,
            ICustomerViewService customerViewService,
            ICustomerAddressViewService customerAddressViewService,
            ICartUrlProvider cartUrlProvider,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider,
            IRecurringCartUrlProvider recurringCartUrlProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            CustomerViewService = customerViewService ?? throw new ArgumentNullException(nameof(customerViewService));
            CustomerAddressViewService = customerAddressViewService ?? throw new ArgumentNullException(nameof(customerAddressViewService));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            RecurringCartUrlProvider = recurringCartUrlProvider ?? throw new ArgumentNullException(nameof(recurringCartUrlProvider));
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

            var urlParam = new BaseUrlParameter { CultureInfo = param.CultureInfo };
            var addressListUrl = MyAccountUrlProvider.GetAddressListUrl(urlParam);
            var changePasswordUrl = MyAccountUrlProvider.GetChangePasswordUrl(urlParam);

            var viewModel = await CustomerViewService.UpdateAccountAsync(param);

            if (viewModel == null) { return Unauthorized(); }

            viewModel.AddressListUrl = addressListUrl;
            viewModel.ChangePasswordUrl = changePasswordUrl;

            return Ok(viewModel);
        }


        [HttpGet]
        [ActionName("addresses")]
        //TODO: Change the method name for GetAdressListAsync or something similar because it confuse people
        public virtual async Task<IHttpActionResult> GetShippingAddressAsync()
        {
            var viewModel = await CustomerAddressViewService.GetAddressListViewModelAsync(new GetAddressListViewModelParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                CountryCode = ComposerContext.CountryCode
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("recurringcartaddresses")]
        public virtual async Task<IHttpActionResult> GetRecurringCartAddressAsync([FromBody]GetRecurringCartAddressRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var cartsUrl = RecurringCartUrlProvider.GetRecurringCartDetailsUrl(new GetRecurringCartDetailsUrlParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                RecurringCartName = request.CartName
            });

            var addAddressUrl = MyAccountUrlProvider.GetAddAddressUrl(new BaseUrlParameter { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = cartsUrl });
            var editAddressBaseUrl = MyAccountUrlProvider.GetUpdateAddressBaseUrl(new BaseUrlParameter { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = cartsUrl });

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
                returnUrl = MyAccountUrlProvider.GetAddressListUrl(new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                });
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
                returnUrl = MyAccountUrlProvider.GetAddressListUrl(new BaseUrlParameter { CultureInfo = ComposerContext.CultureInfo });
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

            if (viewModel == null) { return Unauthorized(); }

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

            if (viewModel == null) { return Unauthorized(); }

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

            if (viewModel == null) { return Unauthorized(); }

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

        [HttpPost]
        [ActionName("recurringorderstemplatesaddresses")]
        public virtual async Task<IHttpActionResult> GetRecurringOrderTemplatesAddressesAsync([FromBody]GetRecurringTemplateAddressRequest request)
        {
            var recurringOrderScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                RecurringScheduleId = request.Id
            });

            var addAddressUrl = MyAccountUrlProvider.GetAddAddressUrl(new BaseUrlParameter 
            { 
                CultureInfo = ComposerContext.CultureInfo, 
                ReturnUrl = recurringOrderScheduleUrl 
            });

            var editAddressBaseUrl = MyAccountUrlProvider.GetUpdateAddressBaseUrl(new BaseUrlParameter 
            { 
                CultureInfo = ComposerContext.CultureInfo, 
                ReturnUrl = recurringOrderScheduleUrl 
            });
            
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
    }
}