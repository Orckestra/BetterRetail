using Moq.AutoMock;
using NUnit.Framework;

namespace Orckestra.Composer.Tests
{
    /// <summary>
    /// Base class for unit tests.
    /// </summary>
    public abstract class BaseTest
    {
        protected AutoMocker Container { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Container = new AutoMocker();
        }
    }
}