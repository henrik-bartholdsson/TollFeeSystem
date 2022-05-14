using System;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var owner = new TollFeeSystem.Core.Types.VehicleOwner() { Name = "Henrik"};

            var vehicle = new TollFeeSystem.Core.Types.Vehicle() { Owner = owner, Brand = "Volvo", RegistrationNumber = "asd-123", VehicleType = Core.StaticData.VehicleType.Diplomat };

            var syst = new TollFeeSystem.Core.TollFeeSystem();


            syst.PassThroughPortal(vehicle, DateTime.Parse("2022-05-14T15:00:00"));
        }

    }
}
