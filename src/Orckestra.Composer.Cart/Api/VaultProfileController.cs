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
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            VaultProfileViewService = vaultProfileViewService ?? throw new ArgumentNullException(nameof(vaultProfileViewService));
            ImageViewService = imageViewService ?? throw new ArgumentNullException(nameof(imageViewService));
        }

        [HttpPost]
        [ActionName("addprofile")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> AddVaultProfile(AddVaultProfileViewModel request)
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

            if (viewModel?.ActivePayment != null)
            {
                viewModel.ActivePayment.CreditCardTrustImage = creditCartTrustImage;

            }

            return Ok(viewModel);
        }
    }
}