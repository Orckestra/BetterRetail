using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateCartRequest
    {
        [Required]
        public Dictionary<string, string> UpdatedCart { get; set; }

        public int? CurrentStep { get; set; }
    }
}
