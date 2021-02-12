using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public interface IUnitConverter
    {
        Task<decimal> Convert(string fromSystemName, string toSystemName, decimal value);
    }
}
