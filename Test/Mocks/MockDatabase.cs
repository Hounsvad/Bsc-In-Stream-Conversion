﻿using Bsc_In_Stream_Conversion;
using Bsc_In_Stream_Conversion.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mocks
{
    public class MockDatabase : IDatabaseAccess
    {
        private List<Unit> db = new List<Unit>()
        {
            new Unit(MockUnits.Meter),
            new Unit(MockUnits.Foot),
            new Unit(MockUnits.Mile),
            new Unit(MockUnits.Kilogram),
            new Unit(MockUnits.Pound),
            new Unit(MockUnits.Second),
            new Unit(MockUnits.Hour),
            new Unit(MockUnits.Kelvin),
            new Unit(MockUnits.Celsius),
            new Unit(MockUnits.Fahrenheit)
        };


        public Task<bool> InsertUnitAsync(Unit unit)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertUnitsAsync(IEnumerable<Unit> units)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> SelectUnit(string SystemName)
        {
            return Task.FromResult(db.Where(u => u.SystemName == SystemName).First());
        }

        public Task<List<Unit>> SelectUnitByUnitName(string UnitName)
        {
            throw new NotImplementedException();
        }
    }
}
