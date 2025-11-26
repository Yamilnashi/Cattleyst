using CattleystData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IDboDbReadContext
    {
        Task<IEnumerable<Location>> LocationList();
    }
}
