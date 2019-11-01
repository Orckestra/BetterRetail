using System.Globalization;
using Moq;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Tests.Mock
{
    internal sealed class ComposerContextFactory
    {
        public static Mock<IComposerRequestContext> Create()
        {
            Mock<IComposerRequestContext> composerContext = new Mock<IComposerRequestContext>(MockBehavior.Strict);

            composerContext
                .SetupGet(c => c.CultureInfo)
                .Returns(CultureInfo.GetCultureInfo("fr-CA"));

            return composerContext;
        }
    }
}
