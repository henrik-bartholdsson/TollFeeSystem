using System;
using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Dto;
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
            Initialize();
        }

        public void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime, int portalId)
        {
            var passTrough = new PassTroughDto
            {
                InputRegNr = vehicleRegistrationNumber,
                PassTroughTime = currentTime,
                PortalId = portalId
            };

            AssignFee(passTrough);
        }
        public IEnumerable<int> GetPortalIds()
        {
            return _TFSContext.Portals.Select(x => x.PortalId).ToList();            
        }
        public IEnumerable<string> GetVehicleRegistrationNumbers()
        {
            var regNumbers = _vehicleRegistry.GetAllVehicleRegistrationNumbers();

            if (regNumbers == null)
                throw new Exception("Log. Connection to vehicle registry failed. Store information and try again later.");

            if (regNumbers.Count == 0)
                throw new Exception("Somone did truncate on the vehicle database, probably my friend Jim...");


            return regNumbers;
        }
        public IEnumerable<FeeHead> GetLicenseHoldersWithFees()
        {
            var a = _TFSContext.FeeHeads;
            return _TFSContext.FeeHeads.Where(x => x.FeeRecords.Count > 0).ToList();
        }





        #region Helpers

        private bool VehicleTypeExceptedFromFee(Vehicle vehicle)
        {
            var exceptedVehicleTypes = _TFSContext.FeeExceptionVehicles;

            foreach (var v in exceptedVehicleTypes)
                if (v.VehicleType == vehicle.VehicleType)
                    return true;

            return false;
        }
        private int GetAmountOfFee(PassTroughDto PassTrough)
        {
            var exceptionDay = DayIsExceptedFromFee(PassTrough.PassTroughTime);
            if (exceptionDay)
                return 0;

            var addressException = ExceptionByAddress(PassTrough);
            if (addressException)
                return 0;



            foreach (var fd in _TFSContext.FeeDefinitions)
            {
                if (PassTrough.PassTroughTime.TimeOfDay >= fd.Start.TimeOfDay &&
                     PassTrough.PassTroughTime.TimeOfDay <= fd.End.TimeOfDay)
                    return fd.Amount;
            }

            return 0;
        }

        private bool ExceptionByAddress(PassTroughDto PassTrough) // Ska korta ned logiken senare.
        {
            var ports = _TFSContext.Portals.Where(
                x =>x.PortalId == PassTrough.PortalId && x.FeeExceptionsByResidentialAddress.Any(
                    x => x == PassTrough.VehicleFromRegistry.Owner.ResidentialAddress
                    )).ToList();

            if (ports != null)
                return true;

            return false;
        }

        private bool DayIsExceptedFromFee(DateTime currentTime) // See exceptions https://www.transportstyrelsen.se/sv/vagtrafik/Trangselskatt/Trangselskatt-i-goteborg/Tider-och-belopp-i-Goteborg/dagar-da-trangselskatt-inte-tas-ut-i-goteborg/
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (currentTime.Month == 6)
                return true;

            return false;
        }

        private void AssignFee(PassTroughDto PassTrough)
        {
            PassTrough.VehicleFromRegistry = _vehicleRegistry.GetVehicleByRegNr(PassTrough.InputRegNr);

            if (PassTrough.VehicleFromRegistry == null)
            {
                // Log for statistics
                return;
            }

            var excepted = VehicleTypeExceptedFromFee(PassTrough.VehicleFromRegistry);
            if (excepted)
                return;

            var amountOfFee = GetAmountOfFee(PassTrough);

            var feeRecord = new FeeRecord
            {
                FeeAmount = amountOfFee,
                FeeTime = PassTrough.PassTroughTime,
                VehicleRegistrationNumber = PassTrough.VehicleFromRegistry.RegistrationNumber
            };

            var vehicleOwner = _TFSContext.FeeHeads.FirstOrDefault(x => x.Name == PassTrough.VehicleFromRegistry.Owner.Name);

            _TFSContext.FeeHeads.Add(
                new FeeHead
                {
                    Name = PassTrough.VehicleFromRegistry.Owner.Name,
                    FeeRecords = new List<FeeRecord> { feeRecord }
                });
        }

        private void Initialize()
        {
            _TFSContext.FeeHeads = new List<FeeHead>();

            _TFSContext.Portals = new List<Portal>();
            _TFSContext.Portals.Add(new Portal { PortalId = 11, PortalNameAddress = "Tingstad North", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 12, PortalNameAddress = "Tingstad South", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 13, PortalNameAddress = "E6 North Entrance", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 14, PortalNameAddress = "E6 North Departure", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 13, PortalNameAddress = "E6 South Entrance", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 14, PortalNameAddress = "E6 South Departure", FeeExceptionsByResidentialAddress = new List<string>() });
            _TFSContext.Portals.Add(new Portal { PortalId = 17, PortalNameAddress = "Backa Entrance", FeeExceptionsByResidentialAddress = new List<string> { "Backa" } });
            _TFSContext.Portals.Add(new Portal { PortalId = 18, PortalNameAddress = "Backa Departure", FeeExceptionsByResidentialAddress = new List<string> { "Backa" } });

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
