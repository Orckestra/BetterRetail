using System.Collections.Generic;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class GroupedLineItemDetailViewModel
    {
        public double Quantity { get; set; }
        public double Total { get; set; }
        public string TopLevelCategoryId { get; set; }
        public string TopLevelCategoryName { get; set; }
        public List<LineItemDetailViewModel> LineItemDetailViewModels { get; set; }
    }
}
