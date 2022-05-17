using System;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Dto
{
    internal class PassTroughDto
    {
        public string InputRegNr { get; set; }
        public DateTime PassTroughTime { get; set; }
        public Vehicle VehicleFromRegistry { get; set; }
        public int PortalId { get; set; }
    }
}
