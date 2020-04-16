using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelSerialization
{
    public class TestViewModelForSerialization : BaseViewModel
    {
        public string TestString { get; set; }

        public Guid TestGuid { get; set; }

        public int TestInt { get; set; }

        public double TestDouble{ get; set; }

        public DateTime TestDateTime { get; set; }

        public Dictionary<string, object> TestBag { get; set; }

        public TestViewModelForSerialization TestNested { get; set; }

    }

    public static class TestViewModelForSerializationFactory
    {
        private static Dictionary<string, object> _bag;
        private static TestViewModelForSerialization _viewModelForSerializationForBag;
        private static TestViewModelForSerialization _nestedViewModelForSerialization;
        private static TestViewModelForSerialization _testViewModelForSerialization;

        public static TestViewModelForSerialization Create()
        {
            _viewModelForSerializationForBag = new TestViewModelForSerialization()
            {
                TestInt = GetRandom.Int(),
                TestString = GetRandom.String(GetRandom.Int(3, 1200)),
                TestGuid = GetRandom.Guid(),
                TestDouble = GetRandom.Double(),
                TestDateTime = GetRandom.DateTime()
            };

            _bag = new Dictionary<string, object>
            {
                { "TestObjectInTheBag", _viewModelForSerializationForBag }, 
                { "TestStringInTheBag", GetRandom.String(GetRandom.Int(3, 1200)) },
                { "TestIntInTheBag", GetRandom.Int(int.MinValue, int.MaxValue) },
                { "TestDateInTheBag", GetRandom.DateTime() },
                { "TestDecimalInTheBag", (decimal) GetRandom.Double() },
                { "TestGuidInTheBag", GetRandom.Guid() },
            };

            _nestedViewModelForSerialization = new TestViewModelForSerialization()
            {
                TestInt = GetRandom.Int(),
                TestString = GetRandom.String(GetRandom.Int(3, 1200)),
                TestGuid = GetRandom.Guid(),
                TestBag = _bag,
                TestDouble = GetRandom.Double(),
                TestDateTime = GetRandom.DateTime()
            };
            _testViewModelForSerialization = new TestViewModelForSerialization()
            {
                TestInt = GetRandom.Int(),
                TestString = GetRandom.String(GetRandom.Int(3, 1200)),
                TestGuid = GetRandom.Guid(),
                TestBag = _bag,
                TestNested = _nestedViewModelForSerialization,
                TestDouble = GetRandom.Double(),
                TestDateTime = GetRandom.DateTime()
            };
            return _testViewModelForSerialization;
        }

    }

}