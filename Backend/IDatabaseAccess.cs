using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public interface IDatabaseAccess
    {
        Task<bool> InsertUnitAsync(Unit unit);
        Task<int> InsertUnitsAsync(IEnumerable<Unit> units);
        Task<Unit> SelectUnit(string SystemName);

        Task<List<Unit>> SelectUnitByUnitName(string UnitName);
    }
}