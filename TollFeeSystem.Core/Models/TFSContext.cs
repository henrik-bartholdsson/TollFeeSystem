using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    internal class TFSContext
    {
        public List<Fee> Fees { get; set; }
        public List<FeeDefinition> FeeDefinitions { get; set; }
        public List<FeeExceptionVehicle> FeeExceptionVehicles { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<LicenseHolder> LicenseHolders { get; set; }

        public TFSContext()
        {
            Fees = new List<Fee>();
            FeeDefinitions = new List<FeeDefinition>();
            FeeExceptionVehicles = new List<FeeExceptionVehicle>();
            Vehicles = new List<Vehicle>();
            LicenseHolders = new List<LicenseHolder>();
        }
    }
}
