using System;

namespace TollFeeSystem.Core.Types
{
    public class FeeRecord
    {
        public int id { get; set; }
        public DateTime FeeTime { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public string ExceptionNote { get; set; }
    }
}
