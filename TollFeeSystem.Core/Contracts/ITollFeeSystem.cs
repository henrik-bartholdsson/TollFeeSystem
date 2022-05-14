using System;
using System.Collections.Generic;
using System.Text;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface ITollFeeSystem
    {
        void PassThroughPortal(Vehicle vehicle, DateTime currentTime);
        IVehicleRegistry GetVehicleRegistry();

        IEnumerable<LicenseHolder> GetLicenseHolders();
    }
}
