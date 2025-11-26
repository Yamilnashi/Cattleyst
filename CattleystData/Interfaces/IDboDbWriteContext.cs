using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IDboDbWriteContext
    {
        Task LocationAdd(string locationName);
        Task LocationUpdate(int locationId, string locationName);
        Task LocationDelete(int locationId);
    }
}
