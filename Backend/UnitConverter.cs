using Bsc_In_Stream_Conversion.Database;
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
        private IDatabaseAccess db;

        public UnitConverter(IDatabaseAccess db)
        {
            this.db = db;
        }

        public async Task<decimal> Convert(string fromSystemName, string toSystemName, decimal value)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
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

        public async Task<decimal> Convert(List<string> fromUnits, List<string> toUnits, decimal value)
        {
            if(fromUnits.Count() != toUnits.Count())
            {
                throw new InvalidOperationException("Not same amount of from and to units");
            }
            var compoundValue = value;
            for(int i = 0; i < fromUnits.Count(); i++)
            {
                Unit fromDatabaseUnit = await db.SelectUnit(fromUnits[i]);
                Unit toDatabaseUnit = await db.SelectUnit(toUnits[i]);
                compoundValue = (((compoundValue * fromDatabaseUnit.ConversionMultiplier) + fromDatabaseUnit.ConversionOffset) - toDatabaseUnit.ConversionOffset) / toDatabaseUnit.ConversionMultiplier;
            }

            return compoundValue;
        }

        //  DEG_F*DEG_F/S -> (((x.m*(((x.m*v+x.o) - y.o)/y.m)+x.o) - y.o)/y.m)     K*K/MINUTE  m/s -> m/h ((v)/0,000621371192)*3600     x/1 m/s -> mile/hr  x -> mile, 1s -> 1hr x/(1/3600)
    }
}
