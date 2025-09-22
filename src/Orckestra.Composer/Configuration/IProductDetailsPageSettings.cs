using System;

namespace Orckestra.Composer.Configuration
{
    public interface IProductDetailsPageSettings
    {
        bool VideosInSummaryEnabled { get;  }
        string DefaultVideoThumbnail {  get; } 
    }
}
