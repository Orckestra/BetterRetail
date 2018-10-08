namespace Orckestra.Composer
{
    public interface IComposerPlugin
    {
        /// <summary>
        /// Register any dependencies, providers, messages that will available to the <see cref="IComposerHost"/>.
        /// </summary>
        /// <param name="host">
        /// The <see cref="IComposerHost"/> instance.
        /// </param>
        void Register(IComposerHost host);
    }
}