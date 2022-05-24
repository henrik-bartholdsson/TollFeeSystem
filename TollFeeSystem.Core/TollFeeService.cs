﻿using Microsoft.EntityFrameworkCore;
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
        }

        public void PassThroughPortal(string vehicleRegistrationNumber, DateTime currentTime, int portalId)
        {
            var passTrough = new PassTroughDto
            {
                InputRegNr = vehicleRegistrationNumber,
                PassTroughTime = currentTime,
                PortalId = portalId
            };

            AssignFeeAsync(passTrough);
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

        private async Task<bool> DateExcepted(DateTime currentTime) // See exceptions https://www.transportstyrelsen.se/sv/vagtrafik/Trangselskatt/Trangselskatt-i-goteborg/Tider-och-belopp-i-Goteborg/dagar-da-trangselskatt-inte-tas-ut-i-goteborg/
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (currentTime.Month == 6)
                return true;

            var customExceptionDays = await _TFSContext.FeeExceptionDays.Where(x => DateTime.Parse(x.Day).Date == currentTime.Date).FirstOrDefaultAsync();
            if (customExceptionDays != null)
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

        private async Task<bool> HasPassedLessThenOneHourAsync(PassTroughDto PassTrough)
        {
            var listOfRecords = await _TFSContext.FeeHeads
                .Where(x => x.Name == PassTrough.VehicleFromRegistry.Owner.Name && x.Day.Date == PassTrough.PassTroughTime.Date)
                .Select(x => x.FeeRecords
                .Where(x => x.FeeTime.Date == PassTrough.PassTroughTime.Date &&
                    x.FeeTime.TimeOfDay > PassTrough.PassTroughTime.AddHours(-StaticData.MaxOneFeePerUnit).TimeOfDay))
                .FirstOrDefaultAsync();

            if(listOfRecords != null)
                if (listOfRecords.Count() > 0)
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
                x.Day.Date == PassTrough.PassTroughTime.Date &&
                x.RegNr == PassTrough.VehicleFromRegistry.RegistrationNumber).FirstOrDefaultAsync();
        }

        private async void AssignFeeAsync(PassTroughDto PassTrough)
        {
            int feeAmount = 0;
            PassTrough.VehicleFromRegistry = _vehicleRegistry.GetVehicleByRegNr(PassTrough.InputRegNr);

            if (PassTrough.VehicleFromRegistry == null)
            {
                // Log for statistics
                return;
            }

            

            var vehicleTypeExcepted = VehicleTypeExceptedAsync(PassTrough.VehicleFromRegistry).Result;
            if (vehicleTypeExcepted)
                return;

            var passedLessThenAnHour = await HasPassedLessThenOneHourAsync(PassTrough);
            if (passedLessThenAnHour)
                return;

            var addressException = AddressExceptedAsync(PassTrough).Result;
            if (addressException)
                PassTrough.ExceptionNote = "A";

            var exceptionDay = DateExcepted(PassTrough.PassTroughTime).Result;
            if (exceptionDay)
                PassTrough.ExceptionNote = "H";

            if (String.IsNullOrEmpty(PassTrough.ExceptionNote))
                feeAmount = GetAmountOfFeeAsync(PassTrough).Result;

            var feeHead = GetFeeHeadAsync(PassTrough).Result;

            if (feeHead != null)
            {
                feeHead.FeeSum = (feeHead.FeeSum + feeAmount) > StaticData.MaxFeePerDay ? StaticData.MaxFeePerDay :
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

       
        #endregion
    }
}
