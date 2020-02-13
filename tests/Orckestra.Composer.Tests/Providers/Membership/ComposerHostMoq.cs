using System;
using System.Reflection;
using Autofac;
using Autofac.Extras.Moq;
using Moq;

namespace Orckestra.Composer.Tests.Providers.Membership
{
    public class ComposerHostMoq : IDisposable
    {
        public AutoMock AutoMock { get; }

        public ComposerHostMoq()
        {
            AutoMock = AutoMock.GetLoose();
            var instance = new Composer.ComposerHost();

            var field = typeof(Composer.ComposerHost).GetField("_rootResolver", BindingFlags.NonPublic | BindingFlags.Instance);
            var scope = AutoMock.Create<ILifetimeScope>();
            field.SetValue(instance, scope);

            var prop = typeof(Composer.ComposerHost).GetProperty("Current");
            prop.SetValue(null, instance);
        }

        public void Dispose()
        {
            if (Composer.ComposerHost.Current != null)
            {
                var field = typeof(Composer.ComposerHost).GetField("_rootResolver", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(Composer.ComposerHost.Current, null);

                var prop = typeof(Composer.ComposerHost).GetProperty("Current");
                prop.SetValue(null, null);
            }
        }
    };
}