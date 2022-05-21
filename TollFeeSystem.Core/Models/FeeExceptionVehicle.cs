using TollFeeSystem.Core.Types;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core.Models
{
    public class FeeExceptionVehicle
    {
        public int Id { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}
