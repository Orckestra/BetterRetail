namespace Orckestra.Composer.Providers.RegionCode
{
    /// <summary>
    /// Represents a Region Code and Postal Code
    /// </summary>
    public abstract class RegionPostalCode
    {
        public string Code { get; set; }

        protected RegionPostalCode(string code)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Represents a Canadian Region Code and Postal Code
    /// </summary>
    public class CanadaRegionPostalCode : RegionPostalCode
    {
        public char PostalCode { get; set; }

        public CanadaRegionPostalCode(string code, char postalCode) : base(code)
        {
            PostalCode = postalCode;
        }
    }

    /// <summary>
    /// Represents a US Region Code and Postal Code
    /// </summary>
    public class UnitedStatesRegionPostalCode : RegionPostalCode
    {
        public int PostalCodeMinRange { get; set; }
        public int PostalCodeMaxRange { get; set; }

        public UnitedStatesRegionPostalCode(string code, int postalCodeMinRange, int postalCodeMaxRange)
            : base(code)
        {
            PostalCodeMinRange = postalCodeMinRange;
            PostalCodeMaxRange = postalCodeMaxRange;
        }
    }
}
