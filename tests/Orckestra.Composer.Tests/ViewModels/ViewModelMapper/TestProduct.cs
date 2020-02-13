using System;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    public class TestProduct
    {
        public double Price { get; set; }
        public string Brand { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public DateTime DateCreated { get; set; }
        public LocalizedString Name { get; set; }
        public PropertyBag PropertyBag { get; set; }

        public TestCategory Category { get; set; }
        public TestProduct[] ChildProducts { get; set; }
        public List<string> Tags { get; set; }
        public string Size { get; set; }
    }
}
