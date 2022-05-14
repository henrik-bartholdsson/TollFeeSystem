using System;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tollFeeSystem = new TollFeeSystem.Core.TollFeeSystem();

            var vehicleRegistry = tollFeeSystem.GetVehicleRegistry();
            var vehicle = vehicleRegistry.GetAllVehicles();

            tollFeeSystem.PassThroughPortal(vehicle[1], DateTime.Parse("2022-05-13T15:00:00"));

            var ownerWithFee = tollFeeSystem.GetLicenseHolders();




            var e = 0;

        }

    }
}
