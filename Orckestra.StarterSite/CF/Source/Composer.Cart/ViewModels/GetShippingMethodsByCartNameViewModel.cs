using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class GetShippingMethodsByCartNameViewModel : BaseViewModel
    {
        public string CartName { get; set; }
    }
}
