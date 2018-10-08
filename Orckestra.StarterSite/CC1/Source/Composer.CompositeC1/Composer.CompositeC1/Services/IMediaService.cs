using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.CompositeC1.Media;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IMediaService
    {
        ImageInfo GetImageInfo(Guid itemId, CultureInfo cultureInfo);
    }
}
