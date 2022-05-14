using System.Collections.Generic;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core.Types
{
    public class VehicleOwner
    {
        public string Name { get; set; }
        public List<Fee> Fees { get; set; }
        public VehicleOwnerType VehicleOwnerType { get; set; }
    }
}
