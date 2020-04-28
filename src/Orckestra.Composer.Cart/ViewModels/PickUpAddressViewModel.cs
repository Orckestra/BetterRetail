using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class PickUpAddressViewModel : BaseViewModel
    {
        /// <summary>
        /// The Store Location Id
        /// </summary>
        public Guid PickUpLocationId { get; set; }
    }
}
