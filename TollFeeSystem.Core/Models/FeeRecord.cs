using System;

namespace TollFeeSystem.Core.Types
{
    public class FeeRecord
    {
        public int id { get; set; }
        public int FeeAmount { get; set; }
        public DateTime FeeTime { get; set; }
        public string VehicleRegistrationNumber { get; set; }
    }
}
