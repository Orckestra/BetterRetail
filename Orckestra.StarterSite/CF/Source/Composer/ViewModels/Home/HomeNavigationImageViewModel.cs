using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class HomeNavigationImageViewModel: BaseViewModel
    {
        public string ImageUrl { get; set; }

        public string ImageUrlTarget { get; set; }

        public string ImageSource { get; set; }

        public string ImageLabel { get; set;  }
        
    }
}
