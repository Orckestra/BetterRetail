using System.Web;

namespace Orckestra.Composer
{
    public class OutputCacheOptions
    {
        public double Duration { get; set; }

        public HttpCacheability HttpCacheability { get; set; }

        public bool SetValidUntilExpires { get; set; }

        public string VaryByHeaders { get; set; }
    }
}