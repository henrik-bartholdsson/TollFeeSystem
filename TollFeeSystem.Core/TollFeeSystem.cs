using System;
using System.Collections.Generic;
using System.Text;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core
{
    public class TollFeeSystem : ITollFeeSystem
    {
        // Main class to interact with the system
        // Use singleton
        // Call portals with vehicle as arguments, the return should may be some kind of message.

        private IVehicleRegistry _vehicleRegistry;
        private TollFeeSystem _tollFeeSystem;
        private List<FeeDefinition> _feeDefinitions; // from transportstyrelsen
        private List<DateTime> _FeeExceptionDays;
        private List<FeeException> _feeExceptionVehicles;
        private List<VehicleOwner> _licenseHolder;

        public TollFeeSystem()
        {
            //_vehicleRegistry = vehicleRegistry;
            //_tollFeeSystem = tollFeeSystem;
            Initializer();
        }

        public void PassThroughPortal(Vehicle vehicle, DateTime currentTime)
        {
            var excepted = VehicleTypeExceptedFromFee(vehicle);
            if (excepted)
                return;

            var amountOfFee = GetAmountOfFee(currentTime);
            var fee = new Fee { FeeAmount = amountOfFee, FeeTime = currentTime, VehicleRegistrationNumber = vehicle.RegistrationNumber };


            var a = 0;
            // Assign Fee on vehicle owner
        }

        public IVehicleRegistry GetVehicleRegistry()
        {
            return _vehicleRegistry;
        }




        private void Initializer()
        {
            _feeDefinitions = new List<FeeDefinition>() {
                new FeeDefinition { Start = DateTime.Parse("06:00:00"), End = DateTime.Parse("06:29:59"), Amount = (int) FeeAmount.Low},
                new FeeDefinition { Start = DateTime.Parse("06:30:00"), End = DateTime.Parse("06:59:59"), Amount = (int) FeeAmount.Medium},
                new FeeDefinition { Start = DateTime.Parse("07:00:00"), End = DateTime.Parse("07:59:59"), Amount = (int) FeeAmount.High},
                new FeeDefinition { Start = DateTime.Parse("08:00:00"), End = DateTime.Parse("08:29:59"), Amount = (int) FeeAmount.Medium },
                new FeeDefinition { Start = DateTime.Parse("08:30:00"), End = DateTime.Parse("14:59:59"), Amount = (int) FeeAmount.Low},
                new FeeDefinition { Start = DateTime.Parse("15:00:00"), End = DateTime.Parse("15:29:59"), Amount = (int) FeeAmount.Medium},
                new FeeDefinition { Start = DateTime.Parse("15:30:00"), End = DateTime.Parse("16:59:59"), Amount = (int) FeeAmount.High},
                new FeeDefinition { Start = DateTime.Parse("17:00:00"), End = DateTime.Parse("17:59:59"), Amount = (int) FeeAmount.Medium},
                new FeeDefinition { Start = DateTime.Parse("18:00:00"), End = DateTime.Parse("18:29:59"), Amount = (int) FeeAmount.Low},
            };

            _feeExceptionVehicles = new List<FeeException>
            {
             new FeeException{ VehicleType = VehicleType.Emergency},
             new FeeException{ VehicleType = VehicleType.Buss},
             new FeeException{ VehicleType = VehicleType.Diplomat},
             new FeeException{ VehicleType = VehicleType.Motorbike},
             new FeeException{ VehicleType = VehicleType.Military}
            };

            _FeeExceptionDays = new List<DateTime>();

            _licenseHolder = new List<VehicleOwner> {
            new VehicleOwner() { Name = "Neo", Fees = new List<Fee>() },
            new VehicleOwner() { Name = "Ripley", Fees = new List<Fee>() },
            new VehicleOwner() { Name = "Elizabeth", Fees = new List<Fee>() },
            new VehicleOwner() { Name = "Luke", Fees = new List<Fee>() },
            new VehicleOwner() { Name = "Bilbo", Fees = new List<Fee>() },
            };


            Vehicle v1 = new Vehicle()
            {
                Brand = "Volvo 142",
                Owner = _licenseHolder[0],
                RegistrationNumber = "ABC-123",
                VehicleType = VehicleType.Personal
            };

            Vehicle v2 = new Vehicle()
            {
                Brand = "Volvo 142",
                Owner = _licenseHolder[1],
                RegistrationNumber = "ABC-123",
                VehicleType = VehicleType.Personal
            };

            Vehicle v3 = new Vehicle()
            {
                Brand = "Volvo 142",
                Owner = _licenseHolder[2],
                RegistrationNumber = "ABC-123",
                VehicleType = VehicleType.Personal
            };

            Vehicle v4 = new Vehicle()
            {
                Brand = "Volvo 142",
                Owner = _licenseHolder[3],
                RegistrationNumber = "ABC-123",
                VehicleType = VehicleType.Personal
            };

            _vehicleRegistry = new VehicleRegistry() { Vehicles = new List<Vehicle> { v1, v2, v3, v4 } };

        }


        #region Helpers

        private bool VehicleTypeExceptedFromFee(Vehicle vehicle)
        {
            foreach(var v in _feeExceptionVehicles)
                if(v.VehicleType == vehicle.VehicleType)
                    return true;

            return false;
        }
        private int GetAmountOfFee(DateTime currentTime)
        {
            var isExcepted = DayIsExceptedFromFee(currentTime);
            if (isExcepted)
                return 0;


            foreach (var fd in _feeDefinitions)
            {
                if (currentTime.TimeOfDay >= fd.Start.TimeOfDay &&
                   currentTime.TimeOfDay <= fd.End.TimeOfDay)
                    return fd.Amount;
            }


            return 0;
        }
        private bool DayIsExceptedFromFee(DateTime currentTime) // See exceptions https://www.transportstyrelsen.se/sv/vagtrafik/Trangselskatt/Trangselskatt-i-goteborg/Tider-och-belopp-i-Goteborg/dagar-da-trangselskatt-inte-tas-ut-i-goteborg/
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (currentTime.Month == 6)
                return true;

            foreach (var day in _FeeExceptionDays)
            {
                if (day.Date == currentTime.Date)
                    return true;
            }

            return false;
        }
        private void AssignFee(Fee fee, Vehicle vehicle)
        {
            vehicle.Owner.AddFee(fee);
        }

        #endregion
    }
}
