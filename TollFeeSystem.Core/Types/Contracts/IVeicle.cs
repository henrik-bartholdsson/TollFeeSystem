using System;
using System.Collections.Generic;
using System.Text;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core.Contracts
{
    public interface IVeicle
    {
        VehicleType GetVehicleType();
    }
}
