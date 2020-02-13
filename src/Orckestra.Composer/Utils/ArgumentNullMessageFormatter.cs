namespace Orckestra.Composer.Utils
{
    public static class ArgumentNullMessageFormatter
    {
        public static string FormatErrorMessage(string propertyName)
        {
            return string.Format("Property {0} of the parameter cannot be null.", propertyName);
        }
    }
}
