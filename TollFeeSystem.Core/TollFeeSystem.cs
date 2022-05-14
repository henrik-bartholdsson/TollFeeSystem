using System;
using System.Collections.Generic;
using System.Linq;
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
        //private TollFeeSystem _tollFeeSystem;
        //private List<FeeDefinition> _feeDefinitions; // from transportstyrelsen
        private List<DateTime> _FeeExceptionDays;
        //private List<FeeExceptionVehicle> _feeExceptionVehicles;
        //private List<LicenseHolder> _licenseHolders;

        private TFSContext _TFSContext;

        public TollFeeSystem()
        {
            //_vehicleRegistry = vehicleRegistry;
            //_tollFeeSystem = tollFeeSystem;
            _TFSContext = new TFSContext();
            Initializer();
        }

        public void PassThroughPortal(Vehicle vehicle, DateTime currentTime)
        {
            var excepted = VehicleTypeExceptedFromFee(vehicle);
            if (excepted)
                return;

            var amountOfFee = GetAmountOfFee(currentTime);
            var fee = new Fee { FeeAmount = amountOfFee, FeeTime = currentTime, VehicleRegistrationNumber = vehicle.RegistrationNumber };

            AssignFee(fee, vehicle);
            var a = 0;
            // Assign Fee on vehicle owner
        }

        public List<Vehicle> GetVehicleRegistry()
        {
            return _TFSContext.Vehicles.ToList();
        }

        public IEnumerable<LicenseHolder> GetLicenseHolders()
        {
            return _TFSContext.LicenseHolders.Where(x => x.Fees.Count > 0).ToList();
        }



        private void Initializer()
        {

            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:00:00"), End = DateTime.Parse("06:29:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:30:00"), End = DateTime.Parse("06:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("07:00:00"), End = DateTime.Parse("07:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:00:00"), End = DateTime.Parse("08:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:30:00"), End = DateTime.Parse("14:59:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:00:00"), End = DateTime.Parse("15:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:30:00"), End = DateTime.Parse("16:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("17:00:00"), End = DateTime.Parse("17:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("18:00:00"), End = DateTime.Parse("18:29:59"), Amount = (int)FeeAmount.Low });


            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle{ VehicleType = VehicleType.Emergency});
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle{ VehicleType = VehicleType.Buss});
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle{ VehicleType = VehicleType.Diplomat});
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle{ VehicleType = VehicleType.Motorbike});
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle{ VehicleType = VehicleType.Military});


            _FeeExceptionDays = new List<DateTime>(); // Not defined yet


            _TFSContext.LicenseHolders.Add(new LicenseHolder() { Name = "Neo", Fees = new List<Fee>() });
            _TFSContext.LicenseHolders.Add(new LicenseHolder() { Name = "Ripley", Fees = new List<Fee>() });
            _TFSContext.LicenseHolders.Add(new LicenseHolder() { Name = "Elizabeth", Fees = new List<Fee>() });
            _TFSContext.LicenseHolders.Add(new LicenseHolder() { Name = "Luke", Fees = new List<Fee>() });
            _TFSContext.LicenseHolders.Add(new LicenseHolder() { Name = "Bilbo", Fees = new List<Fee>() });


            _TFSContext.Vehicles.Add(new Vehicle() { Brand = "Volvo 142", Owner = _TFSContext.LicenseHolders[0], RegistrationNumber = "ABC-123", VehicleType = VehicleType.Personal });
            _TFSContext.Vehicles.Add(new Vehicle() { Brand = "WV Golf GTI", Owner = _TFSContext.LicenseHolders[1], RegistrationNumber = "JSK-983", VehicleType = VehicleType.Personal });
            _TFSContext.Vehicles.Add(new Vehicle() { Brand = "Scoda Fabia", Owner = _TFSContext.LicenseHolders[2], RegistrationNumber = "BWU-081", VehicleType = VehicleType.Personal });
            _TFSContext.Vehicles.Add(new Vehicle() { Brand = "Testa S2", Owner = _TFSContext.LicenseHolders[3], RegistrationNumber = "PCE-592", VehicleType = VehicleType.Personal });
            _TFSContext.Vehicles.Add(new Vehicle() { Brand = "Testa S2", Owner = _TFSContext.LicenseHolders[0], RegistrationNumber = "PCF-591", VehicleType = VehicleType.Personal });
        }


        #region Helpers

        private bool VehicleTypeExceptedFromFee(Vehicle vehicle)
        {
            foreach (var v in _TFSContext.FeeExceptionVehicles)
                if (v.VehicleType == vehicle.VehicleType)
                    return true;

            return false;
        }
        private int GetAmountOfFee(DateTime currentTime)
        {
            var isExcepted = DayIsExceptedFromFee(currentTime);
            if (isExcepted)
                return 0;


            foreach (var fd in _TFSContext.FeeDefinitions)
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
