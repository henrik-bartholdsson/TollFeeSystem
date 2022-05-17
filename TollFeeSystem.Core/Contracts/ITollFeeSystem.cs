using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface ITollFeeSystem
    {
        void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime, int portalId);
        IEnumerable<string> GetVehicleRegistrationNumbers();
        IEnumerable<FeeHead> GetLicenseHoldersWithFees();
        IEnumerable<int> GetPortalIds();
    }
}
