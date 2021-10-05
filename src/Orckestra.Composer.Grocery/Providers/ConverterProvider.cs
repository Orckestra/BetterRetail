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
            switch (fromMeasurement)
            {
                case "Millilitre":
                    mL = number;
                    oz = number * 0.033814;
                    cl = number * 0.1;
                    l = number * 0.001;
                    m = number * 1.0E-6;
                    pt = number * 0.00175975;
                    g = number;
                    kg = number * 0.001;
                    mg = number;
                    break;

                case "Ounce":
                    mL = number * 29.5735;
                    oz = number;
                    cl = number * 2.95735;
                    l = number * 0.0295735;
                    m = number * 2.95735e-5;
                    pt = number * 0.0520421;
                    g = number * 28.3495;
                    kg = number * 0.0283495;
                    mg = number * 28349.5;
                    break;

                case "Centilitre":
                    mL = number * 10;
                    oz = number * 0.33814;
                    cl = number;
                    l = number * 0.01;
                    m = number * 1e-5;
                    pt = number * 0.0175975;
                    g = number * 10;
                    kg = number * 0.01;
                    mg = number * 10000;
                    break;

                case "Litre":
                    mL = number * 1000;
                    oz = number * 33.814;
                    cl = number * 100;
                    l = number;
                    m = number * 0.001;
                    pt = number * 1.75975;
                    g = number * 1000;
                    kg = number * 1;
                    mg = number * 1000000;
                    break;

                case "Metre":
                    mL = number * 1000000;
                    oz = number * 33814;
                    cl = number * 100000;
                    l = number * 1000;
                    m = number;
                    pt = number * 2834.65;
                    g = number * 1000000;
                    kg = number * 10;
                    mg = number * 1000000000;
                    break;

                case "Pint":
                    mL = number * 568.261;
                    oz = number * 19.2152;
                    cl = number * 56.8261;
                    l = number * 0.568261;
                    m = number * 0.000352778;
                    pt = number;
                    g = number * 1E-8;
                    kg = number * 0.473176473;
                    mg = number * 473.176473;
                    break;

                case "Gram":
                    mL = number;
                    oz = number * 0.035274;
                    cl = number * 0.1;
                    l = number * 0.001;
                    m = number * 0.000001;
                    pt = number * 0.00248015873015873;
                    g = number;
                    kg = number * 0.001;
                    mg = number * 1000;
                    break;

                case "Kilogram":
                    mL = number * 1000;
                    oz = number * 35.274;
                    cl = number * 100;
                    l = number * 1;
                    m = number * 0.001;
                    pt = number * 2.113376418865;
                    g = number * 1000;
                    kg = number;
                    mg = number * 1000000;
                    break;

                case "Milligram":
                    mL = number * 0.001;
                    oz = number * 3.5274e-5;
                    cl = number * 0.0001;
                    l = number * 1.0E-6;
                    m = number * 1.0E-6;
                    pt = number * 2.1134E-6;
                    g = number * 0.001;
                    kg = number * 1e-6;
                    mg = number;
                    break;
                case "Unit":
                    unit = number;
                    break;
                case "Pack":
                    pack = number;
                    break;
                case "Each":
                    each = number;
                    break;
            }

            switch (toMeasurement)
            {
                case "Millilitre":
                    return mL;
                case "Ounce":
                    return oz;
                case "Centilitre":
                    return cl;
                case "Litre":
                    return l;
                case "Metre":
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
            }

            return 1;
        }
    }
}
