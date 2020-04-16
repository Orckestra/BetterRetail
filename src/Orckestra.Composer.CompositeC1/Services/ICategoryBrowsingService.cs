using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Services
{
	public interface ICategoryBrowsingService
	{
        void Sync();

        void Clear();
	}
}
