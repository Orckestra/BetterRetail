namespace Orckestra.Composer.Providers.Dam
{
    public static class ConventionBasedDamProviderConfiguration
    {
        private static int _maxThumbnailImages = 4;

        /// <summary>
        /// Gets or sets the maximum number of thumbnail images.
        /// </summary>
        /// <value>
        /// The maximum number of thumbnail images.
        /// </value>
        public static int MaxThumbnailImages
        {
            get { return _maxThumbnailImages; }
            set { _maxThumbnailImages = value; }
        }
    }
}
