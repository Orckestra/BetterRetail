namespace Orckestra.Composer.Services
{
    public interface ICookieAccessor<TDto>
    {
        TDto Read();
        void Write(TDto dto);
        void Clear();
    }
}
