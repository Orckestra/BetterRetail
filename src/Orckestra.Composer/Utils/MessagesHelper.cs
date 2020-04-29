namespace Orckestra.Composer.Utils
{
    public static class MessagesHelper
    {
        public static class ArgumentException
        {
            public static string GetMessageOfNull(string propertyName) => $"Value of {propertyName} cannot be null.";

            public static string GetMessageOfNull() => $"Value cannot be null.";

            public static string GetMessageOfNullWhiteSpace(string propertyName) => $"Value of {propertyName} cannot be null or white space.";

            public static string GetMessageOfNullWhiteSpace() => $"Value cannot be null or white space.";

            public static string GetMessageOfEmpty(string propertyName) => $"Value of {propertyName} cannot be empty.";

            public static string GetMessageOfEmpty() => $"Value cannot be empty.";

            public static string GetMessageOfZeroNegative(string propertyName) => $"Value of {propertyName} cannot be zero or negative.";

            public static string GetMessageOfZeroNegative() => $"Value cannot be zero or negative.";

            public static string GetMessageOfNullEmpty(string propertyName) => $"Value of {propertyName} cannot be null or empty.";

            public static string GetMessageOfNullEmpty() => $"Value cannot be null or empty.";
        }
    }
}