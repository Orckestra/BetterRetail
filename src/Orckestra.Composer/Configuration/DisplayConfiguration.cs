namespace Orckestra.Composer.Configuration
{
    public static class DisplayConfiguration
    {
        public static int ProductNameMaxLength { get; set; }

        static DisplayConfiguration()
        {
            ProductNameMaxLength = 55;
        }
    }
}
