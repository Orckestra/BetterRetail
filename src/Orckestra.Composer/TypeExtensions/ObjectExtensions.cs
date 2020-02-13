using System.Linq;
using System.Reflection;

namespace Orckestra.Composer.TypeExtensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Copy all readable Properties from src to Writable Properties of dst with the same name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        public static void CopyPropertiesFrom<T>(this T dst, T src)
        {
            var plist = src.GetType().GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (PropertyInfo prop in plist)
            {
                prop.SetValue(dst, prop.GetValue(src, null), null);
            }
        }
    }
}
