using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetCheckoutStepUrlParam
    {
        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public int StepNumber { get; set; }
    }
}
