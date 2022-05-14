using System.Collections.Generic;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface IVehicleRegistry
    {
        List<Vehicle> GetAllVehicles();
    }
}
