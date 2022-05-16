using System;

namespace Orckestra.Composer.Cart.Parameters.Order { 

    public class GetCustomerOrderedProductsParam
    {
        public Guid CustomerId { get; set; }
        public string ScopeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MinimumOrderedNumberOfTimes { get; set; }
        public int MaximumItems { get; set; }
    }
}
