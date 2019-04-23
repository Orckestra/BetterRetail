using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Utils;
using Orckestra.Overture;

namespace Orckestra.Composer.Cart.Services
{
    public class RecurringOrderCartsViewService : IRecurringOrderCartsViewService
    {
        protected ICartRepository CartRepository { get; private set; }
        protected IOvertureClient OvertureClient { get; private set; }
        protected IRecurringOrderCartViewModelFactory RecurringOrderCartViewModelFactory { get; private set; }
        protected IImageService ImageService { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IRecurringOrdersRepository RecurringOrdersRepository { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }


        public RecurringOrderCartsViewService(
            ICartRepository cartRepository,
            IOvertureClient overtureClient,
            IRecurringOrderCartViewModelFactory recurringOrderCartViewModelFactory,
            IImageService imageService,
            ILookupService lookupService,
            IRecurringOrdersRepository recurringOrdersRepository,
            IComposerContext composerContext)
        {
            if (cartRepository == null) { throw new ArgumentNullException(nameof(cartRepository)); }
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (recurringOrderCartViewModelFactory == null) { throw new ArgumentNullException(nameof(recurringOrderCartViewModelFactory)); }
            if (imageService == null) { throw new ArgumentNullException(nameof(imageService)); }
            if (lookupService == null) { throw new ArgumentNullException(nameof(lookupService)); }
            if (recurringOrdersRepository == null) { throw new ArgumentNullException(nameof(recurringOrdersRepository)); }
            if (composerContext == null) { throw new ArgumentNullException(nameof(composerContext)); }

            OvertureClient = overtureClient;
            CartRepository = cartRepository;
            RecurringOrderCartViewModelFactory = recurringOrderCartViewModelFactory;
            ImageService = imageService;
            LookupService = lookupService;
            RecurringOrdersRepository = recurringOrdersRepository;
            ComposerContext = composerContext;
        }

        public async Task<RecurringOrderCartsViewModel> GetRecurringOrderCartListViewModelAsync(GetRecurringOrderCartsViewModelParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new RecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCarts(param).ConfigureAwait(false);

            var tasks = carts.Select(pc => CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = pc,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }));
            var viewModels = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new RecurringOrderCartsViewModel
            {
                RecurringOrderCartViewModelList = viewModels.OrderBy(v => v.NextOccurence).ToList(),
            };            
        }

        private async Task<IRecurringOrderCartViewModel> CreateCartViewModelAsync(CreateRecurringOrderCartViewModelParam param)
        {
            var lineItems = param.Cart.GetLineItems();

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
            };

            var methodDisplayNames = await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = param.CultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).ConfigureAwait(false);

            param.PaymentMethodDisplayNames = methodDisplayNames;

            var roProgramNames = lineItems.Select(x => x.RecurringOrderProgramName)
                                                        .Where(l => !string.IsNullOrEmpty(l))
                                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                                        .ToList();
            var programTasks = roProgramNames.Select(programName => RecurringOrdersRepository.GetRecurringOrderProgram(ComposerContext.Scope, programName));
            var programs = await Task.WhenAll(programTasks).ConfigureAwait(false);
            param.RecurringOrderPrograms = programs.ToList();

            var vm = RecurringOrderCartViewModelFactory.CreateRecurringOrderCartViewModel(param);

            return vm;
        }

        public async Task<LightRecurringOrderCartsViewModel> GetLightRecurringOrderCartListViewModelAsync(GetLightRecurringOrderCartListViewModelParam param)
        {
            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return new LightRecurringOrderCartsViewModel();

            var carts = await CartRepository.GetRecurringCarts(new GetRecurringOrderCartsViewModelParam {
                Scope = param.Scope,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                CustomerId = param.CustomerId
            }).ConfigureAwait(false);

            var tasks = carts.Select(pc => CreateLightCartViewModelAsync(new CreateLightRecurringOrderCartViewModelParam
            {
                Cart = pc,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
            }));
            var viewModels = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new LightRecurringOrderCartsViewModel
            {
                RecurringOrderCarts = viewModels.OrderBy(v => v.NextOccurence).ToList(),
            };
        }

        private async Task<LightRecurringOrderCartViewModel> CreateLightCartViewModelAsync(CreateLightRecurringOrderCartViewModelParam param)
        {
            var lineItems = param.Cart.GetLineItems();

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
            };

            var vm = RecurringOrderCartViewModelFactory.CreateLightRecurringOrderCartViewModel(param);

            return vm;
        }
         
        public async Task<IRecurringOrderCartViewModel> GetRecurringOrderCartViewModelAsync(GetRecurringOrderCartViewModelParam param)
        {
            var emptyVm = new CartViewModel();
            var extendedEmptyVm = emptyVm.AsExtensionModel<IRecurringOrderCartViewModel>();

            if (!ConfigurationUtil.GetRecurringOrdersConfigEnabled())
                return extendedEmptyVm;

            if(string.Equals(param.CartName, CartConfiguration.ShoppingCartName, StringComparison.OrdinalIgnoreCase))
                return extendedEmptyVm;

            var cart = await CartRepository.GetCartAsync(new GetCartParam {
                CartName = param.CartName,
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                ExecuteWorkflow = true,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var vm = await CreateCartViewModelAsync(new CreateRecurringOrderCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = false,
                BaseUrl = param.BaseUrl,
            }).ConfigureAwaitWithCulture(false);

            return vm;
        }
    }
}
