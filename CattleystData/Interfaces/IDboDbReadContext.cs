using CattleystData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IDboDbReadContext
    {
        Task<IEnumerable<Location>> LocationList();
        Task<Location> LocationGet(int locationId);
        Task<IEnumerable<Cattle>> CattleList(int[] locationIds = null);
    }
}
