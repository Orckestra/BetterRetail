namespace Orckestra.Composer.Grocery.Providers
{
    public class ConverterProvider: IConverterProvider
    {
        public double ConvertMeasurements(double number, string fromMeasurement, string toMeasurement)
        {
            double mL = 0;
            double oz = 0;
            double cl = 0;
            double l = 0;
            double m = 0;
            double pt = 0;
            double g = 0;
            double kg = 0;
            double mg = 0;
            double each = 0;
            double unit = 0;
            double pack = 0;
            double gallon = 0;
            double lb = 0;
            double flOz = 0;

            switch (fromMeasurement)
            {
                case "Millilitre":
                case "Milliliter":
                    mL = number;
                    flOz = number * 0.033814;
                    cl = number * 0.1;
                    l = number * 0.001;
                    gallon = number * 0.000264172;
                    pt = number * 0.00175975;
                    break;

                case "Ounce":
                    oz = number;
                    lb = number * 0.0625;
                    g = number * 28.3495;
                    kg = number * 0.0283495;
                    mg = number * 28349.5;
                    break;

                case "Centilitre":
                case "Centiliter":
                    mL = number * 10;
                    flOz = number * 0.33814;
                    cl = number;
                    l = number * 0.01;
                    pt = number * 0.0175975;
                    gallon = number * 0.00264172;
                    break;

                case "Litre":
                case "Liter":
                    mL = number * 1000;
                    flOz = number * 33.814;
                    cl = number * 100;
                    l = number;
                    pt = number * 1.75975;
                    gallon = number * 0.264172;
                    break;


                case "Metre":
                case "Meter":
                    m = number;
                    break;

                case "Pint":
                    mL = number * 568.261;
                    flOz = number * 19.2152;
                    cl = number * 56.8261;
                    l = number * 0.568261;
                    pt = number;
                    gallon = number * 0.150119;
                    break;

                case "Gram":
                    oz = number * 0.035274;
                    lb = number * 0.00220462;
                    g = number;
                    kg = number * 0.001;
                    mg = number * 1000;
                    break;

                case "Kilogram":
                    oz = number * 35.274;
                    lb = number * 2.20462;
                    g = number * 1000;
                    kg = number;
                    mg = number * 1000000;
                    break;

                case "Milligram":
                    oz = number * 3.5274e-5;
                    lb = number * 2.20462e-6;
                    g = number * 0.001;
                    kg = number * 1e-6;
                    mg = number;
                    break;
                case "Unit":
                    unit = number;
                    each = number;
                    pack = number;
                    break;
                case "Pack":
                    unit = number;
                    each = number;
                    pack = number;
                    break;
                case "Each":
                    unit = number;
                    each = number;
                    pack = number;
                    break;
                case "Gallon":
                    mL = number * 3785.41;
                    flOz = number * 128;
                    cl = number * 378.541;
                    l = number * 3.78541;
                    gallon = number;
                    pt = number * 6.66139;
                    break;
                case "FluidOunce":
                    mL = number * 29.5735;
                    flOz = number;
                    cl = number * 2.95735;
                    l = number * 0.0295735;
                    gallon = number * 0.0078125;
                    pt = number * 0.0520421;
                    break;
                case "Pounds":
                case "Pound":
                    oz = number * 16;
                    lb = number;
                    g = number * 453.592;
                    kg = number * 0.453592;
                    mg = number * 453592;
                    break;
            }

            switch (toMeasurement)
            {
                case "Millilitre":
                case "Milliliter":
                    return mL;
                case "Ounce":
                    return oz;
                case "Centilitre":
                case "Centiliter":
                    return cl;
                case "Litre":
                case "Liter":
                    return l;
                case "Metre":
                case "Meter":
                    return m;
                case "Pint":
                    return pt;
                case "Gram":
                    return g;
                case "Kilogram":
                    return kg;
                case "Milligram":
                    return mg;
                case "Unit":
                    return unit;
                case "Pack":
                    return pack;
                case "Each":
                    return each;
                case "Gallon":
                    return gallon;
                case "FluidOunce":
                    return flOz;
                case "Pounds":
                case "Pound":
                    return lb;
                default:
                    return 1;
            }
        }
    }
}
