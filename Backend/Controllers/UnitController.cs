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

        public UnitController(IUnitConverter unitConverter)
        {
            this.unitConverter = unitConverter;
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

                var FromUnit = UserUnit.Parse(FromSystemName);
                var ToUnit = UserUnit.Parse(ToSystemName);

                var numeratorValue = await unitConverter.Convert(FromUnit.Numerator, ToUnit.Numerator, Value);
                var denominatorValue = await unitConverter.Convert(FromUnit.Denominator, ToUnit.Denominator, 1);

                var convertedValue = numeratorValue / denominatorValue;

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
