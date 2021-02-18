using Bsc_In_Stream_Conversion.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bsc_In_Stream_Conversion.Lookup.Prefixes;

namespace Bsc_In_Stream_Conversion
{
    public class UserUnit
    {

        //MEGAM/(KG*DEG_C)^    FT^2/LightYear^3 = FT*FT/HR*HR*HR = FT->M 0.308*0.308/3600*3600*3600  U -> SI-Unit -> XU
        public List<string> Numerator { get; private set; } = new List<string>();
        public List<string> Denominator { get; private set; } = new List<string>();

        public ConversionFactor NumeratorPrefixes { get; private set; } = new ConversionFactor(10, 0);
        public ConversionFactor DenominatorPrefixes { get; private set; } = new ConversionFactor(10, 0);

        public static UserUnit Parse(string input)
        {
            var uu = new UserUnit();
            input = input.Replace(" ", "").Replace("\t", "").ToUpperInvariant();

            ParseFractionPart(input.Split("/")[0], uu, true);
            if (input.Contains("/"))
            {
                ParseFractionPart(input.Split("/")[1], uu, false);
            }
            return uu;
        }

        private static void ParseFractionPart(string input, UserUnit uu, bool isNumerator)
        {
            var parts = input.Split("*");
            foreach (var part in parts)
            {
                bool hasPrefix;
                (string, ConversionFactor)? prefixFactor;
                FindPrefix(part, out hasPrefix, out prefixFactor);

                if (part.Contains("^"))
                {
                    SeperatePowers(uu, part, hasPrefix, prefixFactor, isNumerator);
                }
                else
                {
                    string partToAdd = part;
                    if (hasPrefix)
                    {
                        if (isNumerator)
                        {
                            uu.NumeratorPrefixes *= (ConversionFactor)prefixFactor?.Item2;
                        }
                        else
                        {
                            uu.DenominatorPrefixes *= (ConversionFactor)prefixFactor?.Item2;
                        }
                        partToAdd = part.Replace(prefixFactor?.Item1, "");
                    }
                    if (isNumerator)
                    {
                        uu.Numerator.Add(partToAdd);
                    }
                    else
                    {
                        uu.Denominator.Add(partToAdd);
                    }
                    
                }
            }
        }

        private static void SeperatePowers(UserUnit uu, string part, bool hasPrefix, (string, ConversionFactor)? prefixFactor, bool isNumerator)
        {
            var splitPart = part.Split("^");
            var amount = int.Parse(splitPart[1]);
            if (hasPrefix)
            {
                splitPart[0] = splitPart[0].Replace(prefixFactor?.Item1, "");
            }
            for (int i = 0; i < amount; i++)
            {
                if (hasPrefix)
                {
                    if (isNumerator)
                    {
                        uu.NumeratorPrefixes *= (ConversionFactor)prefixFactor?.Item2;
                    }
                    else
                    {
                        uu.DenominatorPrefixes *= (ConversionFactor)prefixFactor?.Item2;
                    }
                }
                if (isNumerator)
                {
                    uu.Numerator.Add(part.Replace(prefixFactor?.Item1, ""));
                }
                else
                {
                    uu.Denominator.Add(part.Replace(prefixFactor?.Item1, ""));
                }
            }
        }

        private static void FindPrefix(string part, out bool hasPrefix, out (string, ConversionFactor)? prefixFactor)
        {
            hasPrefix = false;
            prefixFactor = null;
            foreach (var prefix in Prefixes.IntegerFactor.Keys)
            {
                if (part.StartsWith(prefix))
                {
                    hasPrefix = true;
                    prefixFactor = (prefix, Prefixes.IntegerFactor[prefix]);
                }
            }
        }

    }
}
