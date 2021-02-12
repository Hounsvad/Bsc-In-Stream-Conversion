using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                return Ok(await unitConverter.Convert(FromSystemName, ToSystemName, Value));
            }catch(InvalidOperationException InvEx)
            {
                return BadRequest();
            }
        }
    }
}
