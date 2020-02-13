using Composer.LoadTest.Cookie;
using Orckestra.Composer;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Overture;

namespace Orckestra.Composer.LoadTest
{
    public class TestPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<TestCookieAccessor, ICookieAccessor<TestCookieDto>>(ComponentLifestyle.PerRequest);
            host.Register<TestInventoryLocationProvider, IInventoryLocationProvider>();

            host.RegisterApiControllers(typeof(TestPlugin).Assembly);
        }
    }
}
