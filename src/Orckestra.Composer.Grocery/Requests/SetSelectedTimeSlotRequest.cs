using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Grocery.Requests
{
    public class SetSelectedTimeSlotRequest
    {
        [Required]
        [MinLength(1)]
        public string SlotId { get; set; }

        [Required]
        [MinLength(1)]
        public string Date { get; set; }

        [Required]
        [MinLength(1)]
        public string ShipmentId { get; set; }

        [Required]
        [MinLength(1)]
        public string StoreId { get; set; }
    }
}