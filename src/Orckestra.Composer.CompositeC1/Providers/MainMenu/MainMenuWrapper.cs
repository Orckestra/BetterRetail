using System;

namespace Orckestra.Composer.CompositeC1.Providers.MainMenu
{
    public class MainMenuItemWrapper
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public Nullable<Guid> ParentId { get; set; }
        public string Url { get; set; }
        public Guid Target { get; set; }
        public int Order { get; set; }
        public Nullable<Guid> CssStyle { get; set; }
        public string CssClassName { get; set; }

        public string SourceCultureName { get; set; }
    }
}