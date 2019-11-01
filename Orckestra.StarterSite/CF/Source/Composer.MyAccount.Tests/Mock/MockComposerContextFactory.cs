using System;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Services;
using Orckestra.ForTests;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockComposerContextFactory
    {
        public static Mock<IComposerRequestContext> Create(bool authenticated, bool guest)
        {
            var composerContext = new Mock<IComposerRequestContext>(MockBehavior.Strict);

            composerContext.SetupProperty(p => p.CultureInfo)
                 .Object.CultureInfo = TestingExtensions.GetRandomCulture();

            //composerContext.SetupProperty(p => p.Scope)
             //   .Object.Scope = GetRandom.String(32);

            composerContext.SetupProperty(p => p.CustomerId)
                .Object.CustomerId = guest ? GetRandom.Guid() : Guid.Empty;

            composerContext.SetupProperty(p => p.IsGuest)
                .Object.IsGuest = guest;

            composerContext.SetupGet(p => p.IsAuthenticated)
                .Returns(authenticated);

            return composerContext;
        }
    }
}
