namespace Orckestra.Composer.Grocery.Providers
{
    public interface IConverterProvider
    {
        /// <summary>
        /// Convert Measurements by fromMeasurement and toMeasurement unit of measure 
        /// </summary>
        double ConvertMeasurements(double number, string fromMeasurement, string toMeasurement);
    }
}
