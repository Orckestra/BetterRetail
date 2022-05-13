using Orckestra.Composer.Enums;

namespace Orckestra.Composer.Configuration
{
    public interface IGoogleSettings
    {
        string GTMContainerId { get;  }
        string MapsApiKey { get;  }
        int MapsZoomLevel { get; }
        int MapsMarkerPadding { get;  }
        LengthMeasureUnitEnum LengthMeasureUnit { get; }
        decimal? StoresAvailabilityDistance { get; }
    }
}
