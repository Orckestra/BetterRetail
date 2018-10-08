﻿using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CompleteCheckoutViewModel : BaseViewModel
    {
        public string OrderNumber { get; set; }

        public string CustomerEmail { get; set; }

        public string NextStepUrl { get; set; }

        public string Affiliation { get; set; }

        public decimal? Revenu { get; set; }

        public decimal? Tax { get; set; }

        public decimal? Shipping { get; set; }

        public List<CouponViewModel> Coupons { get; set; }

        public string Coupon { get; set; }

        public string ShippingOptions { get; set; }

        public string BillingCurrency { get; set; }

        public List<CompleteCheckoutLineItemViewModel> LineItems { get; set; }

        public CompleteCheckoutViewModel()
        {
            LineItems = new List<CompleteCheckoutLineItemViewModel>();
            Coupons = new List<CouponViewModel>();
        }
    }
}