using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Dto;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    internal class TFSContextOld
    {
        public List<FeeRecord> Fees { get; set; }
        public List<PortalDto> Portals { get; set; }
        public List<FeeHead> FeeHeads { get; set; }
        public List<DateTime> FeeExceptionDays { get; set; }
        public List<FeeDefinition> FeeDefinitions { get; set; }
        public List<FeeExceptionVehicle> FeeExceptionVehicles { get; set; }

        public TFSContextOld()
        {
            Fees = new List<FeeRecord>();
            Portals = new List<PortalDto>();
            FeeHeads = new List<FeeHead>();
            FeeDefinitions = new List<FeeDefinition>();
            FeeExceptionDays = new List<DateTime> { DateTime.Now };
            FeeExceptionVehicles = new List<FeeExceptionVehicle>();
        }
    }
}
