using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.VehicleRegistry.Models
{
    internal class VRContext
    {
        public List<Vehicle> Vehicles { get; set; }
        public List<LicenseHolder> LicenseHolders { get; set; }
    }
}
