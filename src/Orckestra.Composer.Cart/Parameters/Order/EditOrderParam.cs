using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class EditOrderParam
    {
        [Required(AllowEmptyStrings = false)]
        public string OrderNumber { get; set; }
    }
}
