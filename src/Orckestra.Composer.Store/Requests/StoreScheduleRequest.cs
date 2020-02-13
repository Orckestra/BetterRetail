using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.Store.Requests
{
    public class StoreScheduleRequest
    {
        [Required]
        [MinLength(1)]
        public string FulfillmentLocationId { get; set; }
    }
}
