namespace Orckestra.Composer.ViewModels
{
    public sealed class PageHeaderViewModel : BaseViewModel
    {
        public string PageTitle { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public bool NoIndex { get; set; }

        public string CanonicalUrl { get; set; }
    }
}
