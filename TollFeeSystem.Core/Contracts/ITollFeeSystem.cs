using System;
using System.Collections.Generic;
using System.Text;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface ITollFeeSystem
    {
        void PassThroughPortal(Vehicle vehicle, DateTime currentTime);
        List<Vehicle> GetVehicleRegistry();

        IEnumerable<LicenseHolder> GetLicenseHolders();
    }
}
