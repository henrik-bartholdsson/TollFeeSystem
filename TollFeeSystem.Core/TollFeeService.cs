using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TollFeeSystem.Core.Dto;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core
{
    public class TollFeeService : ITollFeeService
    {
        private IVehicleRegistryService _vehicleRegistry;
        private TfsContext _TFSContext;

        public TollFeeService(IVehicleRegistryService vehicleRegistry, TfsContext TfsContext)
        {
            _vehicleRegistry = vehicleRegistry;
            _TFSContext = TfsContext;
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

        public IEnumerable<FeeHead> GetLicenseHoldersWithFees()
        {
            var a = _TFSContext.FeeHeads.Where(x => x.FeeRecords.Count > 0).ToList(); // Remove
            return _TFSContext.FeeHeads.Where(x => x.FeeRecords.Count > 0).ToList();
        }

        public async Task<IEnumerable<Portal>> GetPortalsAsync()
        {
            return await _TFSContext.Portals.Where(x => x != null).ToListAsync();
        }



        #region Helpers

        private int GetAmountOfFee(PassTroughDto PassTrough)
        {
            var exceptionDay = DateExcepted(PassTrough.PassTroughTime);
            if (exceptionDay)
                return 0;


            var addressException = AddressExcepted(PassTrough);
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

        private bool AddressExcepted(PassTroughDto PassTrough) // Ska korta ned logiken senare.
        {
            var exAddresses = _TFSContext.FeeExceptionsByResidentialAddresses.Where(x => x != null).ToList();

            var ports = _TFSContext.Portals.Where(
                x => x.Id == PassTrough.PortalId && x.FeeExceptionsByResidentialAddress.Any(
                    x => x.Address == PassTrough.VehicleFromRegistry.Owner.ResidentialAddress
                    )).ToList();

            if (ports == null)
                return false;

            if (ports.Count == 0)
                return false;

            return true;
        }

        private bool DateExcepted(DateTime currentTime) // See exceptions https://www.transportstyrelsen.se/sv/vagtrafik/Trangselskatt/Trangselskatt-i-goteborg/Tider-och-belopp-i-Goteborg/dagar-da-trangselskatt-inte-tas-ut-i-goteborg/
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (currentTime.Month == 6)
                return true;

            return false;
        }

        private bool VehicleTypeExcepted(Vehicle vehicle)
        {
            var exceptedVehicleTypes = _TFSContext.FeeExceptionVehicles;

            foreach (var v in exceptedVehicleTypes)
                if (v.VehicleType == vehicle.VehicleType)
                    return true;

            return false;
        }
        private async Task<bool> HasPassedLessThenOneHourAsync(PassTroughDto PassTrough) // Ska föränkla
        {
            var head = await _TFSContext.FeeHeads.Where(
                x => x.Name == PassTrough.VehicleFromRegistry.Owner.Name)
                .Include(x => x.FeeRecords).FirstOrDefaultAsync();

            if (head == null)
                return false;

            var records = head.FeeRecords.Where(x => x.FeeTime.Date == PassTrough.PassTroughTime.Date &&
            x.FeeTime.TimeOfDay > PassTrough.PassTroughTime.AddHours(-1).TimeOfDay).ToList();

            if (records != null)
                if (records.Count > 0)
                    return true;

            return false;
        }

        private async void AssignFee(PassTroughDto PassTrough)
        {
            PassTrough.VehicleFromRegistry = _vehicleRegistry.GetVehicleByRegNr(PassTrough.InputRegNr);

            if (PassTrough.VehicleFromRegistry == null)
            {
                // Log for statistics
                return;
            }

            var excepted = VehicleTypeExcepted(PassTrough.VehicleFromRegistry);
            if (excepted)
                return;

            var lessThenAnHour = await HasPassedLessThenOneHourAsync(PassTrough);
            if (lessThenAnHour)
                return;

            var amountOfFee = GetAmountOfFee(PassTrough);

            var feeRecord = new FeeRecord
            {
                FeeAmount = amountOfFee,
                FeeTime = PassTrough.PassTroughTime,
                VehicleRegistrationNumber = PassTrough.VehicleFromRegistry.RegistrationNumber
            };


            foreach (var h in _TFSContext.FeeHeads)
            {
                if (h.Name == PassTrough.VehicleFromRegistry.Owner.Name)
                {
                    h.FeeRecords.Add(feeRecord);
                    return;
                }
            }




            _TFSContext.FeeHeads.Add(
                new FeeHead
                {
                    Name = PassTrough.VehicleFromRegistry.Owner.Name,
                    FeeRecords = new List<FeeRecord> { feeRecord }
                });

            _TFSContext.SaveChanges();
        }

        private void Initialize()
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

            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Emergency });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Buss });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Diplomat });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Motorbike });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Military });

            _TFSContext.FeeExceptionDays.Add(new FeeExceptionDay { Day = "2022-12-24" });

            var feeExceptionsByResidentialAddress1 = new List<FeeExceptionsByResidentialAddress>()
            { new FeeExceptionsByResidentialAddress { Address = "Backa" } };

            var feeExceptionsByResidentialAddress2 = new List<FeeExceptionsByResidentialAddress>()
            { new FeeExceptionsByResidentialAddress { Address = "Backa" } };

            _TFSContext.Portals.Add(new Portal() { Id = 11, PortalNameAddress = "Tingstad North" });
            _TFSContext.Portals.Add(new Portal() { Id = 12, PortalNameAddress = "Tingstad South" });
            _TFSContext.Portals.Add(new Portal() { Id = 13, PortalNameAddress = "E6 North Entrance" });
            _TFSContext.Portals.Add(new Portal() { Id = 14, PortalNameAddress = "E6 North Departure" });
            _TFSContext.Portals.Add(new Portal() { Id = 15, PortalNameAddress = "E6 South Entrance" });
            _TFSContext.Portals.Add(new Portal() { Id = 16, PortalNameAddress = "E6 South Departure" });
            _TFSContext.Portals.Add(new Portal()
            {
                Id = 17,
                PortalNameAddress = "Backa Entrance",
                FeeExceptionsByResidentialAddress = feeExceptionsByResidentialAddress1
            });
            _TFSContext.Portals.Add(new Portal()
            {
                Id = 18,
                PortalNameAddress = "Backa Departure",
                FeeExceptionsByResidentialAddress = feeExceptionsByResidentialAddress2
            });

            _TFSContext.SaveChanges();
        }



        #endregion
    }
}
