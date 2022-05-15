using System;
using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core
{
    public class TollFeeSystem : ITollFeeSystem
    {
        // Use singleton

        private IVehicleRegistry _vehicleRegistry;

        private TFSContext _TFSContext;

        public TollFeeSystem()
        {
            _vehicleRegistry = new VehicleRegistry();
            _TFSContext = new TFSContext();
            Initializer();
        }

        public void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime)
        {
            AssignFee(currentTime, vehicleRegistrationNumber);
        }





        public IEnumerable<string> GetAllVehicleRegistrationNumbers()
        {
            var regNumbers = _vehicleRegistry.GetAllVehicleRegistrationNumbers();

            if (regNumbers == null)
                throw new Exception("Log connection to vehicle registry failed. Store information and try again later.");

            if (regNumbers.Count == 0)
                throw new Exception("Somone did truncate on the vehicle database, probably my friend Jim...");


            return regNumbers;
        }


        public IEnumerable<FeeHead> GetLicenseHoldersThatHaveFees()
        {
            var a = _TFSContext.FeeHeads;
            return _TFSContext.FeeHeads.Where(x => x.FeeRecords.Count > 0).ToList();
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

            return false;
        }
        private void AssignFee(DateTime currentTime, string vehicleRegistrationNumber)
        {
            var vehicle = _vehicleRegistry.GetVehicleByRegNr(vehicleRegistrationNumber);

            if (vehicle == null)
            {
                // Inform authorities 
            }

            var excepted = VehicleTypeExceptedFromFee(vehicle);
            if (excepted)
                return;

            var amountOfFee = GetAmountOfFee(currentTime);
            var feeRecord = new FeeRecord { FeeAmount = amountOfFee, FeeTime = currentTime, VehicleRegistrationNumber = vehicle.RegistrationNumber };

            var vehicleOwner = _TFSContext.FeeHeads.FirstOrDefault(x => x.Name == vehicle.Owner.Name);

            //if(vehicleOwner == null)
            //{
            //    _TFSContext.FeeHeads = new List<FeeHead>();
            //}

            _TFSContext.FeeHeads.Add(new FeeHead { Name = vehicle.Owner.Name, FeeRecords = new List<FeeRecord> { feeRecord } });
        }
        private void Initializer()
        {
            _TFSContext.FeeHeads = new List<FeeHead>();

            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:00:00"), End = DateTime.Parse("06:29:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:30:00"), End = DateTime.Parse("06:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("07:00:00"), End = DateTime.Parse("07:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:00:00"), End = DateTime.Parse("08:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:30:00"), End = DateTime.Parse("14:59:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:00:00"), End = DateTime.Parse("15:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:30:00"), End = DateTime.Parse("16:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("17:00:00"), End = DateTime.Parse("17:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("18:00:00"), End = DateTime.Parse("18:29:59"), Amount = (int)FeeAmount.Low });


            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Emergency });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Buss });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Diplomat });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Motorbike });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Military });

            _TFSContext.FeeExceptionDays.Add(DateTime.Parse("2022-12-24"));
        }


        #endregion
    }
}
