using System;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class GetSelectedTimeSlotParam
    {
        /// <summary>
        /// The id of the store
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }
    }
}
