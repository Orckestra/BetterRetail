namespace Orckestra.Composer.ViewModels
{
    public sealed class ImageViewModel : BaseViewModel
    {
        /// <summary>
        /// The Url of the Image.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The Alt info of the Image.
        /// </summary>
        public string Alt { get; set; }
    }
}
