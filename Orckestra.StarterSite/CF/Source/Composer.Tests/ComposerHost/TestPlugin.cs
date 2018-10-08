namespace Orckestra.Composer.Tests.ComposerHost
{
    public class TestPlugin : IComposerPlugin
    {
        public static bool WasDiscovered;

        public void Register(IComposerHost host)
        {
            WasDiscovered = true;
        }
    }
}
