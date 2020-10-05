using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Grocery.Requests
{
    public class GetTimeSlotsAvailabilityRequest
    {
        [Required]
        [MinLength(1)]
        public string StoreId { get; set; }

        [Required]
        [MinLength(1)]
        public string FulfillmentMethodTypeString { get; set; }

        [Required]
        [MinLength(1)]
        public string ShipmentId { get; set; }
    }
}