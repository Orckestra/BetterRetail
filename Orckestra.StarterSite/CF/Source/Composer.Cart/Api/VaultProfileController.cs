using System;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Extensions;
using Orckestra.Composer.Services;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class VaultProfileController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IVaultProfileViewService VaultProfileViewService { get; private set; }
        protected IImageViewService ImageViewService { get; private set; }

        public VaultProfileController(IComposerContext composerContext, IVaultProfileViewService vaultProfileViewService,
            IImageViewService imageViewService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (vaultProfileViewService == null) { throw new ArgumentNullException("vaultProfileViewService"); }
            if (imageViewService == null) { throw new ArgumentNullException("imageViewService"); }

            ComposerContext = composerContext;
            VaultProfileViewService = vaultProfileViewService;
            ImageViewService = imageViewService;
        }

        [HttpPost]
        [ActionName("addprofile")]
        [ValidateModelState]
        public async Task<IHttpActionResult> AddVaultProfile(AddVaultProfileViewModel request)
        {
            if (request == null) { return BadRequest("Request body cannot be empty."); }

            var addCreditCardParam = new AddCreditCardParam
            {
                CardHolderName = request.CardHolderName,
                CartName = CartConfiguration.ShoppingCartName,
                CustomerId = ComposerContext.CustomerId,
                ScopeId = ComposerContext.Scope,
                PaymentId = request.PaymentId.GetValueOrDefault(),
                VaultTokenId = request.VaultTokenId,
                IpAddress = Request.GetClientIp(),
                CultureInfo = ComposerContext.CultureInfo,
                CreatePaymentProfile = request.CreatePaymentProfile,
                PaymentProviderName = request.PaymentProviderName
            };

            var creditCartTrustImage = ImageViewService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);
            var viewModel = await VaultProfileViewService.AddCreditCardAsync(addCreditCardParam);

            if (viewModel != null && viewModel.ActivePayment != null)
            {
                viewModel.ActivePayment.CreditCardTrustImage = creditCartTrustImage;

            }

            return Ok(viewModel);
        }
    }
}
