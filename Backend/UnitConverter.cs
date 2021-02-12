using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class UnitConverter : IUnitConverter
    {
        public async Task<decimal> Convert(string fromSystemName, string toSystemName, decimal value)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var db = new DatabaseAccess();
            Unit fromUnit = await db.SelectUnit(fromSystemName);
            Unit toUnit = await db.SelectUnit(toSystemName);

            if (!fromUnit.DimensionVector.Equals(toUnit.DimensionVector))
            {
                throw new InvalidOperationException("Units cannot be converted");
            }

            decimal baseValue = (value * fromUnit.ConversionMultiplier) + fromUnit.ConversionOffset;
            decimal newValue = (baseValue - toUnit.ConversionOffset) / toUnit.ConversionMultiplier;

            timer.Stop();
            Log.Information($"Conversion from {value} {fromSystemName} to {newValue} {toSystemName} took: [{timer.ElapsedMilliseconds}]"); 
            return newValue;
        }
    }
}
