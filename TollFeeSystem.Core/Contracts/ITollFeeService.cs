using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Types.Contracts
{
    public interface ITollFeeService
    {
        void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime, int portalId);
        Task<IEnumerable<FeeHead>> GetLicenseHoldersWithFeesAwait();
        Task<IEnumerable<Portal>> GetPortalsAsync();
    }
}
