using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class CartViewModelFactoryMock
    {
        internal static Mock<ICartViewModelFactory> Create()
        {
            var mock = new Mock<ICartViewModelFactory>();

            mock.Setup(m => m.CreateCartViewModel(It.IsNotNull<CreateCartViewModelParam>()))
                .Returns((CreateCartViewModelParam p) => CreateCart(p));

            return mock;
        }

        internal static Mock<ICartViewModelFactory> MockGetPaymentMethodViewModel()
        {
            var mock = new Mock<ICartViewModelFactory>();

            mock.Setup(m => m.GetPaymentMethodViewModel(It.IsNotNull<PaymentMethod>(), It.IsAny<Dictionary<string, string>>(), It.IsNotNull<CultureInfo>()))
                .Returns((PaymentMethod pm, Dictionary<string, string> displayNames, CultureInfo ci) =>
                {
                    var vm = new PaymentMethodViewModel
                    {
                        Id = pm.Id,                        
                        PaymentProviderName = pm.PaymentProviderName,
                        PaymentType = pm.Type.ToString(),
                    };

                    return vm;
                });

            return mock;
        }

        internal static Mock<ICartViewModelFactory> MockGetShippingMethodViewModel()
        {
            var mock = new Mock<ICartViewModelFactory>();

            mock.Setup(m => m.GetShippingMethodViewModel(It.IsNotNull<FulfillmentMethod>(), It.IsNotNull<CultureInfo>()))
                .Returns((FulfillmentMethod fm, CultureInfo ci) =>
                {
                    var vm = new ShippingMethodViewModel
                    {
                        Cost = fm.Cost.ToString("C2"),
                        CostDouble = fm.Cost,
                        DisplayName = fm.Name,
                        Name = fm.Name,
                        ShippingProviderId = fm.ShippingProviderId,
                        ExpectedDaysBeforeDelivery = GetRandom.String(5)
                    };

                    return vm;
                });

            return mock;
        } 

        private static CartViewModel CreateCart(CreateCartViewModelParam param)
        {
            if (param.Cart == null || param.CultureInfo == null || param.ProductImageInfo == null) return null;
            
            return new CartViewModel
            {
                Coupons = GetApplicableCoupons(param),
                ShippingAddress = CreateFakeAddress()
            };
        }

        private static AddressViewModel CreateFakeAddress()
        {
            return new AddressViewModel
            {
                PostalCode = GetRandom.String(32),
                CountryCode = GetRandom.String(32),
                RegionCode = GetRandom.String(32)
            };
        }

        private static CouponsViewModel GetApplicableCoupons(CreateCartViewModelParam param)
        {
            if (param.Cart.Coupons == null) { return null; }

            var applicationCoupons = param.Cart.Coupons.Where(c => c.CouponState == CouponState.Ok);

            return new CouponsViewModel()
            {
                ApplicableCoupons = applicationCoupons.Select(ac=>new CouponViewModel()
                {
                    CouponCode = ac.CouponCode,
                    DisplayText = ac.DisplayText
                }).ToList(),
                Messages = new List<CartMessageViewModel>()
            };
        }
    }
}
