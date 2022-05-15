using System;
using System.Collections.Generic;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface ITollFeeSystem
    {
        void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime);
        List<Vehicle> GetVehicleRegistry();

        IEnumerable<LicenseHolder> GetLicenseHoldersThatHaveFees();
    }
}
