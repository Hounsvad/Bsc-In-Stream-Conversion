using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class UserUnit
    {

        //MEGAM/(KG*DEG_C)^    FT^2/LightYear^3 = FT*FT/HR*HR*HR = FT->M 0.308*0.308/3600*3600*3600  U -> SI-Unit -> XU
        public List<string> Numerator { get; private set; } = new List<string>();
        public List<string> Denominator { get; private set; } = new List<string>();


        public static UserUnit Parse(string input)
        {
            var up = new UserUnit();
            input = input.Replace(" ", "").Replace("\t", "").ToUpperInvariant();

            ParseFractionPart(input.Split("/")[0], up.Numerator);
            if (input.Contains("/"))
            {
                ParseFractionPart(input.Split("/")[1], up.Denominator);
            }
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
