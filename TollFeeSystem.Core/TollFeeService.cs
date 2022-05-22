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



        #region Used only by simulation sequence
        public async Task<IEnumerable<FeeHead>> GetLicenseHoldersWithFeesAwait()
        {
            return await _TFSContext.FeeHeads.Where(x => x != null).ToListAsync();
        }

        public async Task<IEnumerable<Portal>> GetPortalsAsync()
        {
            return await _TFSContext.Portals.Where(x => x != null).ToListAsync();
        }

        #endregion





        #region Private helpers

        private async Task<int> GetAmountOfFeeAsync(PassTroughDto PassTrough)
        {
            var feeDefinitions = await _TFSContext.FeeDefinitions.Where(x => x != null).ToListAsync();

            foreach (var fd in feeDefinitions)
            {
                if (PassTrough.PassTroughTime.TimeOfDay >= fd.Start.TimeOfDay &&
                     PassTrough.PassTroughTime.TimeOfDay <= fd.End.TimeOfDay)
                    return fd.Amount;
            }

            return 0;
        }

        private async Task<bool> AddressExceptedAsync(PassTroughDto PassTrough)
        {
            var ports = await _TFSContext.Portals.Where(
                x => x.Id == PassTrough.PortalId && x.FeeExceptionsByResidentialAddress.Any(
                    x => x.Address == PassTrough.VehicleFromRegistry.Owner.ResidentialAddress
                    )).ToListAsync();

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

        private async Task<bool> VehicleTypeExceptedAsync(Vehicle vehicle)
        {
            var exceptedVehicleTypes = await _TFSContext.FeeExceptionVehicles.ToListAsync();

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

        private async void SaveFeeHeadAsync(FeeHead feeHead)
        {
            _TFSContext.FeeHeads.Update(feeHead);
            await _TFSContext.SaveChangesAsync();
        }

        private async Task<FeeHead> GetFeeHeadAsync(PassTroughDto PassTrough)
        {
            return await _TFSContext.FeeHeads.Where(x =>
                x.Name == PassTrough.VehicleFromRegistry.Owner.Name &&
                x.Day.Day == PassTrough.PassTroughTime.Day &&
                x.RegNr == PassTrough.VehicleFromRegistry.RegistrationNumber).FirstOrDefaultAsync();
        }

        private async void AssignFee(PassTroughDto PassTrough)
        {
            PassTrough.VehicleFromRegistry = _vehicleRegistry.GetVehicleByRegNr(PassTrough.InputRegNr); // Async in a real case.

            if (PassTrough.VehicleFromRegistry == null)
            {
                // Log for statistics
                return;
            }

            var addressException = AddressExceptedAsync(PassTrough).Result;
            if (addressException)
                PassTrough.ExceptionNote = "A";

            var exceptionDay = DateExcepted(PassTrough.PassTroughTime);
            if (exceptionDay)
                PassTrough.ExceptionNote = "H";

            var vehicleTypeExcepted = VehicleTypeExceptedAsync(PassTrough.VehicleFromRegistry).Result;
            if (vehicleTypeExcepted)
                return;

            var passedLessThenAnHour = await HasPassedLessThenOneHourAsync(PassTrough);
            if (passedLessThenAnHour)
                return;

            var feeAmount = GetAmountOfFeeAsync(PassTrough).Result;

            var feeHead = GetFeeHeadAsync(PassTrough).Result;

            if (feeHead != null)
            {
                feeHead.FeeSum = (feeHead.FeeSum + feeAmount) > 60 ? 60 :
                    feeHead.FeeSum += feeAmount;

                feeHead.FeeRecords.Add(
                    new FeeRecord
                    {
                        FeeTime = PassTrough.PassTroughTime,
                        VehicleRegistrationNumber = PassTrough.VehicleFromRegistry.RegistrationNumber,
                        ExceptionNote = PassTrough.ExceptionNote,
                    });
            }
            else
            {
                feeHead = new FeeHead()
                {
                    Day = PassTrough.PassTroughTime,
                    FeeSum = feeAmount,
                    Name = PassTrough.VehicleFromRegistry.Owner.Name,
                    RegNr = PassTrough.VehicleFromRegistry.RegistrationNumber,
                    FeeRecords = new List<FeeRecord>()
                { new FeeRecord {
                    FeeTime = PassTrough.PassTroughTime,
                    VehicleRegistrationNumber = PassTrough.VehicleFromRegistry.RegistrationNumber,
                    ExceptionNote = PassTrough.ExceptionNote
                } }
                };
            }

            SaveFeeHeadAsync(feeHead);
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
