using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class DimensionVector
    {
        public short AmountOfSubstance { get; set; }
        public short ElectricCurrent { get; set; }
        public short Length { get; set; }
        public short LuminousIntensity{ get; set; }
        public short Mass { get; set; }
        public short Temperature{ get; set; }
        public short Time { get; set; }
        public short Dimensionless { get; set; }

        public DimensionVector()
        {
        }

        public DimensionVector(short amountOfSubstance, short electricCurrent, short length, short luminousIntensity, short mass, short temperature, short time)
        {
            AmountOfSubstance = amountOfSubstance;
            ElectricCurrent = electricCurrent;
            Length = length;
            LuminousIntensity = luminousIntensity;
            Mass = mass;
            Temperature = temperature;
            Time = time;
        }

        public static DimensionVector Parse(string s)
        {
            var v = new DimensionVector();
            var indexOfE = s.IndexOf("E");
            var indexOfL = s.IndexOf("L");
            var indexOfI = s.IndexOf("I");
            var indexOfM = s.IndexOf("M");
            var indexOfH = s.IndexOf("H");
            var indexOfT = s.IndexOf("T");
            var indexOfD = s.IndexOf("D");

            v.AmountOfSubstance = short.Parse(s.Substring(1, indexOfE - 1));
            v.ElectricCurrent = short.Parse(s.Substring(indexOfE + 1, indexOfL - indexOfE - 1));
            v.Length = short.Parse(s.Substring(indexOfL + 1, indexOfI - indexOfL - 1));
            v.LuminousIntensity = short.Parse(s.Substring(indexOfI + 1, indexOfM - indexOfI - 1));
            v.Mass = short.Parse(s.Substring(indexOfM + 1, indexOfH - indexOfM - 1));
            v.Temperature = short.Parse(s.Substring(indexOfH + 1, indexOfT - indexOfH - 1));
            v.Time = short.Parse(s.Substring(indexOfT + 1, indexOfD - indexOfT - 1));
            v.Dimensionless = short.Parse(s.Substring(indexOfD + 1));

            return v;
        }

        public override bool Equals(object obj)
        {
            if(obj is DimensionVector)
            {
                var v = (DimensionVector)obj;
                return AmountOfSubstance == v.AmountOfSubstance &&
                    ElectricCurrent == v.ElectricCurrent &&
                    Length == v.Length &&
                    LuminousIntensity == v.LuminousIntensity &&
                    Mass == v.Mass &&
                    Temperature == v.Temperature &&
                    Time == v.Time &&
                    Dimensionless == v.Dimensionless;
            }
            else
            {
                return false;
            }
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AmountOfSubstance, ElectricCurrent, Length, LuminousIntensity, Mass, Temperature, Time, Dimensionless);
        }
    }
}
