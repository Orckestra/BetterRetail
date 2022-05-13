using System;
using System.Linq;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Providers.RegionCode
{
    public class RegionCodeProvider : IRegionCodeProvider
    {
        private CanadaRegionPostalCode[] _canadaPostalCodes;
        private UnitedStatesRegionPostalCode[] _usPostalCodes;

        public RegionCodeProvider()
        {
            BuildCanadaRegion();
            BuildUsRegion();
        }

        public string GetRegion(string code, string countryCode)
        {
            if (countryCode == CountryCodes.Canada)
            {
                return GetCanadaRegion(code, countryCode);
            }
            else if (countryCode == CountryCodes.UnitedStates)
            {
                return GetUnitedStatesRegion(code, countryCode);
            }
            throw new ArgumentException("This country is not supported");
        }

        private string GetUnitedStatesRegion(string code, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(code)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(code)); }

            var succcessful = int.TryParse(code, out int zipCode);

            if (!succcessful)
            {
                throw new ArgumentException("The zip code is in an invalid format");
            }

            var state = (
                from usPostalCode
                in _usPostalCodes
                where zipCode >= usPostalCode.PostalCodeMinRange && zipCode <= usPostalCode.PostalCodeMaxRange
                select usPostalCode.Code).Distinct().SingleOrDefault();

            return MergeRegions(countryCode, state);
        }

        private string GetCanadaRegion(string postalCode, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(postalCode)); }
            
            var firstChar = postalCode[0];
            //if char is lowercase?
            if (!firstChar.IsAlpha())
            {
                throw new ArgumentException("The postal code should begin with a letter");
            }

            var region = (
                from canadaPostalCode
                in _canadaPostalCodes
                where canadaPostalCode.PostalCode == firstChar
                select canadaPostalCode.Code).FirstOrDefault();

            return MergeRegions(countryCode, region);
        }

        private void BuildCanadaRegion()
        {
            _canadaPostalCodes = new CanadaRegionPostalCode[19]
            {
                new CanadaRegionPostalCode("NL", 'A'),
                new CanadaRegionPostalCode("NS", 'B'),
                new CanadaRegionPostalCode("PE", 'C'),
                new CanadaRegionPostalCode("NB", 'E'),
                new CanadaRegionPostalCode("QC", 'G'),
                new CanadaRegionPostalCode("QC", 'H'),
                new CanadaRegionPostalCode("QC", 'J'),
                new CanadaRegionPostalCode("ON", 'K'),
                new CanadaRegionPostalCode("ON", 'L'),
                new CanadaRegionPostalCode("ON", 'M'),
                new CanadaRegionPostalCode("ON", 'N'),
                new CanadaRegionPostalCode("ON", 'P'),
                new CanadaRegionPostalCode("MB", 'R'),
                new CanadaRegionPostalCode("SK", 'S'),
                new CanadaRegionPostalCode("AB", 'T'),
                new CanadaRegionPostalCode("BC", 'V'),
                ////Possible problem here postal codes beginning with X is for both
                ////Nunavut and Northwest territories. Fortunately because they are
                ////regions there's no provincial tax. Only the federal tax apply
                ////which is equal for every province or regions which is 5% for now.
                new CanadaRegionPostalCode("NU", 'X'),
                new CanadaRegionPostalCode("NT", 'X'),
                new CanadaRegionPostalCode("YT", 'Y')
            };
        }

        private void BuildUsRegion()
        {
            _usPostalCodes = new UnitedStatesRegionPostalCode[65]
            {
                new UnitedStatesRegionPostalCode("AL", 35000, 36999),
                new UnitedStatesRegionPostalCode("AK", 99500, 99999),
                new UnitedStatesRegionPostalCode("AR", 71600, 72999),
                new UnitedStatesRegionPostalCode("AR", 75500, 75599),
                new UnitedStatesRegionPostalCode("AZ", 85000, 86599),
                new UnitedStatesRegionPostalCode("CA", 90000, 96699),
                new UnitedStatesRegionPostalCode("CO", 80000, 81699),
                new UnitedStatesRegionPostalCode("CT", 06000, 06299),
                new UnitedStatesRegionPostalCode("CT", 06400, 06999),
                new UnitedStatesRegionPostalCode("DC", 20000, 20599),
                new UnitedStatesRegionPostalCode("DE", 19700, 19999),
                new UnitedStatesRegionPostalCode("FL", 32000, 34999),
                new UnitedStatesRegionPostalCode("FM", 96900, 96999),
                new UnitedStatesRegionPostalCode("GA", 30000, 31999),
                new UnitedStatesRegionPostalCode("GA", 39800, 39999),
                new UnitedStatesRegionPostalCode("GU", 96900, 96999),
                new UnitedStatesRegionPostalCode("HI", 96700, 96899),
                new UnitedStatesRegionPostalCode("IA", 50000, 52899),
                new UnitedStatesRegionPostalCode("ID", 83200, 83899),
                new UnitedStatesRegionPostalCode("IL", 60000, 62999),
                new UnitedStatesRegionPostalCode("IN", 46000, 47999),
                new UnitedStatesRegionPostalCode("KS", 66000, 67999),
                new UnitedStatesRegionPostalCode("KY", 40000, 42799),
                new UnitedStatesRegionPostalCode("LA", 70000, 71499),
                new UnitedStatesRegionPostalCode("MA", 01000, 02799),
                new UnitedStatesRegionPostalCode("MA", 05500, 05599),
                new UnitedStatesRegionPostalCode("MD", 20600, 21999),
                new UnitedStatesRegionPostalCode("ME", 03900, 04999),
                new UnitedStatesRegionPostalCode("MI", 48000, 49999),
                new UnitedStatesRegionPostalCode("MN", 55000, 56799),
                new UnitedStatesRegionPostalCode("MO", 63000, 65899),
                new UnitedStatesRegionPostalCode("MS", 38600, 39799),
                new UnitedStatesRegionPostalCode("MT", 59000, 59999),
                new UnitedStatesRegionPostalCode("NC", 26900, 28999),
                new UnitedStatesRegionPostalCode("ND", 58000, 58899),
                new UnitedStatesRegionPostalCode("NE", 68000, 69399),
                new UnitedStatesRegionPostalCode("NH", 03030, 03999),
                new UnitedStatesRegionPostalCode("NJ", 07000, 08999),
                new UnitedStatesRegionPostalCode("NM", 87000, 88499),
                new UnitedStatesRegionPostalCode("NV", 88900, 89899),
                new UnitedStatesRegionPostalCode("NY", 00500, 00599),
                new UnitedStatesRegionPostalCode("NY", 06300, 06399),
                new UnitedStatesRegionPostalCode("NY", 09000, 14999),
                new UnitedStatesRegionPostalCode("OH", 43000, 45999),
                new UnitedStatesRegionPostalCode("OK", 73000, 74999),
                new UnitedStatesRegionPostalCode("OR", 97000, 97999),
                new UnitedStatesRegionPostalCode("PA", 15000, 19699),
                new UnitedStatesRegionPostalCode("PR", 00600, 00999),
                new UnitedStatesRegionPostalCode("RI", 02800, 02999),
                new UnitedStatesRegionPostalCode("SC", 29000, 29999),
                new UnitedStatesRegionPostalCode("SD", 57000, 57799),
                new UnitedStatesRegionPostalCode("TN", 37000, 38599),
                new UnitedStatesRegionPostalCode("TX", 75000, 75499),
                new UnitedStatesRegionPostalCode("TX", 75600, 79999),
                new UnitedStatesRegionPostalCode("TX", 88500, 88599),
                new UnitedStatesRegionPostalCode("UT", 84000, 84799),
                new UnitedStatesRegionPostalCode("VA", 20100, 20199),
                new UnitedStatesRegionPostalCode("VA", 22000, 24699),
                new UnitedStatesRegionPostalCode("VI", 00800, 00899),
                new UnitedStatesRegionPostalCode("VT", 05000, 05499),
                new UnitedStatesRegionPostalCode("VT", 05600, 05699),
                new UnitedStatesRegionPostalCode("WA", 98000, 99499),
                new UnitedStatesRegionPostalCode("WI", 53000, 54999),
                new UnitedStatesRegionPostalCode("WV", 24700, 26899),
                new UnitedStatesRegionPostalCode("WY", 82000, 83199),
            };
        }

        private string MergeRegions(string countryCode, string regionCode)
        {
            return string.Concat(countryCode, "-", regionCode);
        }
    }

    public static class Extension
    {
        public static bool IsAlpha(this char c)
        {
            return (c >= 'A') && (c <= 'Z');
        }
    }
}
