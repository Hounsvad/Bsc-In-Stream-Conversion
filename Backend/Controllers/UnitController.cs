using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitConverter unitConverter;
        private readonly UnitFactory unitFactory;

        public UnitController(IUnitConverter unitConverter, UnitFactory unitFactory)
        {
            this.unitConverter = unitConverter;
            this.unitFactory = unitFactory;
        }

        [HttpGet("{FromSystemName}/{ToSystemName}/{Value}")]
        public async Task<IActionResult> ConvertTo(string FromSystemName, string ToSystemName, decimal Value)
        {
            if(String.IsNullOrWhiteSpace(FromSystemName) || String.IsNullOrWhiteSpace(ToSystemName))
            {
                return NotFound();
            }

            try
            {
                FromSystemName = FromSystemName.Replace("47", "/");
                ToSystemName = ToSystemName.Replace("47", "/");

                var FromUnit = await unitFactory.Parse(FromSystemName);
                var ToUnit = await unitFactory.Parse(ToSystemName);

                var convertedValue = ToUnit.ConvertFromBaseValue(FromUnit.ConvertToBaseValue(Value));

                return Ok(convertedValue);
            }catch(InvalidOperationException InvEx)
            {
                return BadRequest();
            }catch(Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace);
                return Problem();
            }
        }
    }
}
