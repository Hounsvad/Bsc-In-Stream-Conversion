﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class Unit
    {
        public string UnitName { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public decimal ConversionMultiplier { get; set; } = -1;
        public decimal ConversionOffset { get; set; }

        public List<string> QuantityKinds = new List<string>();
        public DimensionVector DimensionVector { get; set; }
    }
}