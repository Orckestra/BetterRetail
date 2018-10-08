using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels.Home
{
    public interface IMenuEntryViewModel
    {
         string DisplayName { get; set; }
         string Url { get; set; }
         string UrlTarget { get; set; }
         MenuTypeEnum MenuType { get; set; }
    }
}
