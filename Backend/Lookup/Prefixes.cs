using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion.Lookup
{
    public class Prefixes
    {
        public struct ConversionFactor
        {
            int Base;
            int Factor;

            public ConversionFactor(int @base, int factor)
            {
                Base = @base;
                Factor = factor;
            }
        }

        public static Dictionary<string, ConversionFactor> IntegerFactor = new Dictionary<string, ConversionFactor>()
        {
            {"YOTTA", new ConversionFactor(10, 24)},
            {"ZETTA", new ConversionFactor(10, 21)},
            {"EXA", new ConversionFactor(10, 18) },
            {"PETA", new ConversionFactor(10, 15) },
            {"TERA", new ConversionFactor(10, 12) },
            {"GIGA", new ConversionFactor(10, 9) },
            {"MEGA", new ConversionFactor(10, 6) },
            {"KILO", new ConversionFactor(10, 3) },
            {"HECTO", new ConversionFactor(10, 2) },
            {"DECA", new ConversionFactor(10, 1) },
            {"DECI", new ConversionFactor(10, -1) },
            {"CENTI", new ConversionFactor(10, -2) },
            {"MILLI", new ConversionFactor(10, -3) },
            {"MICRO", new ConversionFactor(10, -6) },
            {"NANO", new ConversionFactor(10, -9) },
            {"PICO", new ConversionFactor(10, -12) },
            {"FEMTO", new ConversionFactor(10, -15) },
            {"ATTO", new ConversionFactor(10, -18) },
            {"ZEPTO", new ConversionFactor(10, -21) },
            {"YOCTO", new ConversionFactor(10, -24) },
            {"KIBI", new ConversionFactor(2, 10) },
            {"MIBI", new ConversionFactor(2, 20) },
            {"GIBI", new ConversionFactor(2, 30) },
            {"TEBI", new ConversionFactor(2, 40) },
            {"PEBI", new ConversionFactor(2, 50) },
            {"EXBI", new ConversionFactor(2, 60) },
            {"ZEBI", new ConversionFactor(2, 70) },
            {"YOBI", new ConversionFactor(2, 80) }
        };
    }
}
