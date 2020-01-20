namespace Orckestra.Composer.HandlebarsCompiler.Services
{
    public interface IHandlebarsCompileService
    {
        void PrecompileHandlebarsTemplate(string compiledFile, string[] handlebarFiles);
        void PrecompileHandlebarsTemplate(string compiledFile, string templatesPath);
    }
}
