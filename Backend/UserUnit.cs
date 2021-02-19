﻿using Bsc_In_Stream_Conversion.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bsc_In_Stream_Conversion.Lookup.Prefixes;

namespace Bsc_In_Stream_Conversion
{
    public class UserUnit
    {
        public string NumeratorName { get; private set; } = "";
        public string DenominatorName { get; private set; } = "";

        public string Name { get {
                var factor = NumeratorPrefix * DenominatorPrefix;
                return Prefixes.IntegerFactor.Where(x => x.Value == factor).First().Key + NumeratorName + "/" + DenominatorName;
            } }
        //MEGAM/(KG*DEG_C)^    FT^2/LightYear^3 = FT*FT/HR*HR*HR = FT->M 0.308*0.308/3600*3600*3600  U -> SI-Unit -> XU
        public List<UserUnit> Numerator { get; private set; } = new List<UserUnit>();
        public List<UserUnit> Denominator { get; private set; } = new List<UserUnit>();

        public ConversionFactor NumeratorPrefix { get; private set; } = new ConversionFactor(10, 0);
        public ConversionFactor DenominatorPrefix { get; private set; } = new ConversionFactor(10, 0);

        public decimal PrefixFactor { get
            {
                return (decimal)Math.Pow(NumeratorPrefix.Base, NumeratorPrefix.Factor) / (decimal)Math.Pow(DenominatorPrefix.Base, DenominatorPrefix.Factor);
            } 
        }

        public decimal Multiplier = 1;
        public decimal OffSet = 0;

        public UserUnit()
        {
        }

        public UserUnit(string name, ConversionFactor numeratorPrefixes, ConversionFactor denominatorPrefixes, decimal multiplier, decimal offSet)
        {
            NumeratorName = name;
            NumeratorPrefix = numeratorPrefixes;
            DenominatorPrefix = denominatorPrefixes;
            Multiplier = multiplier;
            OffSet = offSet;
        }

        public decimal ConvertToBaseValue(decimal value)
        {
            return (value * Multiplier + OffSet) * PrefixFactor;
        }

        public decimal ConvertFromBaseValue(decimal value)
        {
            return (value - OffSet) / Multiplier / PrefixFactor;
        }

        public static UserUnit operator *(UserUnit uu1, UserUnit uu2)
        {
            var uu = new UserUnit();
            if (!String.IsNullOrWhiteSpace(uu1.NumeratorName))
            {
                uu.NumeratorName = String.IsNullOrWhiteSpace(uu.NumeratorName)? uu1.NumeratorName : uu.NumeratorName + "*" + uu1.NumeratorName;
            }
            if (!String.IsNullOrWhiteSpace(uu2.NumeratorName))
            {
                uu.NumeratorName = String.IsNullOrWhiteSpace(uu.NumeratorName) ? uu2.NumeratorName : uu.NumeratorName + "*" + uu2.NumeratorName;
            }
            if (!String.IsNullOrWhiteSpace(uu1.DenominatorName))
            {
                uu.DenominatorName = String.IsNullOrWhiteSpace(uu.DenominatorName) ? uu1.DenominatorName : uu.DenominatorName + "*" + uu1.DenominatorName;
            }
            if (!String.IsNullOrWhiteSpace(uu2.DenominatorName))
            {
                uu.DenominatorName = String.IsNullOrWhiteSpace(uu.DenominatorName) ? uu2.DenominatorName : uu.DenominatorName + "*" + uu2.DenominatorName;
            }
            uu.Numerator.AddRange(uu1.Numerator);
            uu.Numerator.AddRange(uu2.Numerator);
            uu.Denominator.AddRange(uu1.Denominator);
            uu.Denominator.AddRange(uu2.Denominator);
            uu.Multiplier = uu1.Multiplier * uu2.Multiplier;
            uu.OffSet = uu1.OffSet * uu2.Multiplier + uu2.OffSet;
            uu.NumeratorPrefix = uu1.NumeratorPrefix * uu2.NumeratorPrefix;
            uu.DenominatorPrefix = uu1.DenominatorPrefix * uu2.DenominatorPrefix;
            return uu;
        }

        public static UserUnit operator /(UserUnit uu1, UserUnit uu2)
        {
            var uu = new UserUnit();
            if (!String.IsNullOrWhiteSpace(uu1.NumeratorName))
            {
                uu.NumeratorName = String.IsNullOrWhiteSpace(uu.NumeratorName) ? uu1.NumeratorName : uu.NumeratorName + "*" + uu1.NumeratorName;
            }
            if (!String.IsNullOrWhiteSpace(uu2.DenominatorName))
            {
                uu.NumeratorName = String.IsNullOrWhiteSpace(uu.NumeratorName) ? uu2.DenominatorName : uu.NumeratorName + "*" + uu2.DenominatorName;
            }
            if (!String.IsNullOrWhiteSpace(uu1.DenominatorName))
            {
                uu.DenominatorName = String.IsNullOrWhiteSpace(uu.DenominatorName) ? uu1.DenominatorName : uu.DenominatorName + "*" + uu1.DenominatorName;
            }
            if (!String.IsNullOrWhiteSpace(uu2.NumeratorName))
            {
                uu.DenominatorName = String.IsNullOrWhiteSpace(uu.DenominatorName) ? uu2.NumeratorName : uu.DenominatorName + "*" + uu2.NumeratorName;
            }
            uu.Numerator.AddRange(uu1.Numerator);
            uu.Numerator.AddRange(uu2.Denominator);
            uu.Denominator.AddRange(uu1.Denominator);
            uu.Denominator.AddRange(uu2.Numerator);
            uu.Multiplier = uu1.Multiplier / (uu2.Multiplier + uu2.OffSet);
            uu.OffSet = uu1.OffSet / (uu2.Multiplier + uu2.OffSet);
            uu.NumeratorPrefix = uu1.NumeratorPrefix * uu2.DenominatorPrefix;
            uu.DenominatorPrefix = uu1.DenominatorPrefix * uu2.NumeratorPrefix;
            return uu;
        }
    }
}
