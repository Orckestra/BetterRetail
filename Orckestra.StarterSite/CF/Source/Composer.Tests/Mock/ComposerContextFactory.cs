using System.Globalization;
using Moq;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Tests.Mock
{
    internal sealed class ComposerContextFactory
    {
        public static Mock<IComposerContext> Create()
        {
            Mock<IComposerContext> composerContext = new Mock<IComposerContext>(MockBehavior.Strict);

            composerContext
                .SetupGet(c => c.CultureInfo)
                .Returns(CultureInfo.GetCultureInfo("fr-CA"));

            return composerContext;
        }
    }
}
