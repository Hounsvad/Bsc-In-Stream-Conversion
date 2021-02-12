using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion.Model
{
    public class Conversion
    {
        public Unit FromUnit;
        public Unit ToUnit;



        public decimal GetFactor(decimal value)
        {
            return (((value * FromUnit.ConversionMultiplier) + FromUnit.ConversionOffset )- ToUnit.ConversionOffset) / ToUnit.ConversionMultiplier;
        }
    }
}
