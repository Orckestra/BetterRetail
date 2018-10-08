using Moq.AutoMock;
using NUnit.Framework;

namespace Orckestra.Composer.Product.Tests
{
    public class BaseTest
    {
        public AutoMocker Container { get; set; }
        [SetUp]
        public virtual void Setup()
        {
            Container = new AutoMocker();
        }
    }
}
