using System;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.Providers.MonerisPayment.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    public class VaultProfileViewService : IVaultProfileViewService
    {
        protected IVaultProfileRepository VaultProfileRepository { get; private set; }
        protected IViewModelMapper ViewModelMapper { get; private set; }
        protected IPaymentProviderFactory PaymentProviderFactory { get; private set; }


        public VaultProfileViewService(IVaultProfileRepository vaultProfileRepository, IViewModelMapper viewModelMapper, 
            IPaymentProviderFactory paymentProviderFactory)
        {
            if (vaultProfileRepository == null) { throw new ArgumentNullException("vaultProfileRepository"); }
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }
            if (paymentProviderFactory == null) { throw new ArgumentNullException("paymentProviderFactory"); }

            VaultProfileRepository = vaultProfileRepository;
            ViewModelMapper = viewModelMapper;
            PaymentProviderFactory = paymentProviderFactory;
        }

        public async Task<MonerisAddVaultProfileViewModel> AddCreditCardAsync(AddCreditCardParam addCreditCardParam)
        {
            var model = await VaultProfileRepository.AddCreditCardAsync(addCreditCardParam).ConfigureAwait(false);
            var vm = MapModelToViewModel(model, addCreditCardParam.CultureInfo);

            return vm;
        }

        protected virtual MonerisAddVaultProfileViewModel MapModelToViewModel(VaultProfileCreationResult model,
            CultureInfo cultureInfo)
        {
            var vm = ViewModelMapper.MapTo<MonerisAddVaultProfileViewModel>(model, cultureInfo);

            vm.ActivePayment = MapActivePayment(model.UpdatedPayment, cultureInfo);
            return vm;
        }

        protected virtual ActivePaymentViewModel MapActivePayment(Payment activePayment, CultureInfo cultureInfo)
        {
            var paymentProvider = PaymentProviderFactory.ResolveProvider(activePayment.PaymentMethod.PaymentProviderName);
            var activePaymentVm = ViewModelMapper.MapTo<ActivePaymentViewModel>(activePayment, cultureInfo);

            activePaymentVm.ShouldCapturePayment = paymentProvider.ShouldCapturePayment(activePayment);
            if (activePaymentVm.ShouldCapturePayment)
            {
                activePaymentVm.CapturePaymentUrl = paymentProvider.GetCapturePaymentUrl(activePayment, cultureInfo);
            }

            activePaymentVm.ProviderName = paymentProvider.ProviderName;
            activePaymentVm.ProviderType = paymentProvider.ProviderType;

            paymentProvider.AugmentViewModel(activePaymentVm, activePayment);
            return activePaymentVm;
        }
    }
}
