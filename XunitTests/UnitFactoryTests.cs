using Bsc_In_Stream_Conversion;
using Bsc_In_Stream_Conversion.Database;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Mocks;
using Xunit;

namespace Test
{
    public class UnitFactoryTests
    {
        [Theory]
        [InlineData("M", 1, 0)]
        [InlineData("S", 1, 0)]
        [InlineData("FT", 0.3048, 0)]
        [InlineData("DEG_C", 1, 273.15)]
        [InlineData("M/S", 1, 0)]
        [InlineData("MI/HR", 0.44704, 0)]
        [InlineData("KG*M", 1, 0)]
        [InlineData("LB*FT", 0.45359237, 0)]
        [InlineData("DEG_C/HR", 0.0002777777777777777777777778, 0.075875)]
        public async Task CanParseInput(string input, decimal multiplier, decimal offset)
        {
            IDatabaseAccess db = new MockDatabase();
            var unitFactory = new UnitFactory(db);
            var unit = await unitFactory.Parse(input);
            Math.Abs(unit.Multiplier - multiplier).Should().BeLessThan(0.0000000001m);
            Math.Abs(unit.OffSet - offset).Should().BeLessThan(0.0000000001m);
        }
    }
}
