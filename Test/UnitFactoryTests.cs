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
    class UnitFactoryTests
    {
        [Theory]
        [InlineData("M", 1, 0)]
        public async Task CanParseInput(string input, decimal multiplier, decimal offset)
        {
            IDatabaseAccess db = new MockDatabase();
            var unitFactory = new UnitFactory(db);
            var unit = await unitFactory.Parse(input);
            unit.Multiplier.Should().Be(multiplier);
            unit.OffSet.Should().Be(offset);
        }
    }
}
