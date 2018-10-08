namespace Hangfire.CompositeC1
{
    public static class GlobalConfigurationExtensions
    {
        public static CompositeC1Storage UseCompositeC1Storage(this IGlobalConfiguration configuration)
        {
            var storageOptions = new CompositeC1StorageOptions();

            return configuration.UseCompositeC1Storage(storageOptions);
        }

        public static CompositeC1Storage UseCompositeC1Storage(this IGlobalConfiguration configuration, CompositeC1StorageOptions storageOptions)
        {
            var storage = new CompositeC1Storage(storageOptions);

            configuration.UseStorage(storage);

            return storage;
        }
    }
}
