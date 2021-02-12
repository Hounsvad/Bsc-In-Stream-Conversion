using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class UnitParser
    {

        //MEGAM/(KG*DEG_C)^    FT^2/LightYear^3 = FT*FT/HR*HR*HR = FT->M 0.308*0.308/3600*3600*3600  U -> SI-Unit -> XU
        private List<string> Numerator = new List<string>();
        private List<string> Denominator = new List<string>();

        private decimal SI_UnitFactor = 0.0m;

        private List<string> SI_Numerator = new List<string>();
        private List<string> SI_Denominator = new List<string>();

        public static UnitParser Parse(string input)
        {
            var up = new UnitParser();
            input = input.Replace(" ", "").Replace("\t", "").ToUpperInvariant();

            ParseFractionPart(input.Split("/")[0], up.Numerator);
            ParseFractionPart(input.Split("/")[1], up.Denominator);
            return up;
        }

        private static void ParseFractionPart(string input, List<string> list)
        {
            var numeratorPart = input.Split("*");
            foreach (var part in numeratorPart)
            {
                if (part.Contains("^"))
                {
                    var splitPart = part.Split("^");
                    var amount = int.Parse(splitPart[1]);
                    for (int i = 0; i < amount; i++)
                    {
                        list.Add(splitPart[0]);
                    }
                }
                else
                {
                    list.Add(part);
                }
            }
        }


    }
}
