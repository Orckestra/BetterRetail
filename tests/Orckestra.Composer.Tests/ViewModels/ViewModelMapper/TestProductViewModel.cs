using System.Collections.Generic;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public class TestProductViewModel : BaseViewModel
    {
        public double Price { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        [Formatting("TestCategory","Date")]
        public string DateCreated { get; set; }
        public string Colour { get; set; }
        public string Description { get; set; }
        public TestCategoryViewModel Category { get; set; }
        public TestCategoryViewModel CustomCategory { get; set; }
        public TestProductViewModel[] ChildProducts { get; set; }
        public List<string> Tags { get; set; }
        [Lookup(LookupType.Product, "Size")]
        public string Size { get; set; }
    }
}