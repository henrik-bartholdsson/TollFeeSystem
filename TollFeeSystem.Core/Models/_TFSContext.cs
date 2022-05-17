using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    internal class TFSContext
    {
        public List<FeeRecord> Fees { get; set; }
        public List<Portal> Portals { get; set; }
        public List<FeeHead> FeeHeads { get; set; }
        public List<DateTime> FeeExceptionDays { get; set; }
        public List<FeeDefinition> FeeDefinitions { get; set; }
        public List<FeeExceptionVehicle> FeeExceptionVehicles { get; set; }

        public TFSContext()
        {
            Fees = new List<FeeRecord>();
            Portals = new List<Portal>();
            FeeHeads = new List<FeeHead>();
            FeeDefinitions = new List<FeeDefinition>();
            FeeExceptionDays = new List<DateTime> { DateTime.Now };
            FeeExceptionVehicles = new List<FeeExceptionVehicle>();
        }
    }
}
