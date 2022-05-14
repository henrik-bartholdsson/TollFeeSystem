using System;

namespace TollFeeSystem.Core.Types
{
    public class Fee
    {
        public int FeeAmount { get; set; }
        public DateTime FeeTime { get; set; }
        public string VehicleRegistrationNumber { get; set; }
    }
}
