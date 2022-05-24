using System;
using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Dto;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core.Service
{
    public class TollFeeService : ITollFeeService
    {
        private IVehicleRegistryService _vehicleRegistry;
        private IUnitOfWork _IUnitOfWork;

        public TollFeeService(IVehicleRegistryService vehicleRegistry, IUnitOfWork UnitOfWork)
        {
            _vehicleRegistry = vehicleRegistry;
            _IUnitOfWork = UnitOfWork;
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
        public IEnumerable<FeeHead> GetLicenseHoldersWithFees()
        {
            return _IUnitOfWork.FeeHeadRepository.GetAll().Result.Where(x => x != null).ToList();
        }

        public IEnumerable<Portal> GetPortals()
        {
            return _IUnitOfWork.PortalRepository.GetAll().Result.Where(x => x != null).ToList();
        }

        #endregion





        #region Private helpers

        private int GetAmountOfFee(PassTroughDto PassTrough)
        {
            var feeDefinitions = _IUnitOfWork.FeeDefinitionRepository.GetAll().Result.Where(x => x != null).ToList();

            foreach (var fd in feeDefinitions)
            {
                if (PassTrough.PassTroughTime.TimeOfDay >= fd.Start.TimeOfDay &&
                     PassTrough.PassTroughTime.TimeOfDay <= fd.End.TimeOfDay)
                    return fd.Amount;
            }

            return 0;
        }

        private bool AddressExcepted(PassTroughDto PassTrough)
        {
            // Nedan rader är fult, ska göra bättre
            var portal = _IUnitOfWork.PortalRepository.Get(PassTrough.PortalId).Result;

            if (portal.FeeExceptionsByResidentialAddress == null)
                return false;

            var excepted = portal.FeeExceptionsByResidentialAddress.Where(x => x != null && x.Address == PassTrough.VehicleFromRegistry.Owner.ResidentialAddress).FirstOrDefault();

            if (excepted == null)
                return false;

            return true;
        }

        private bool DateExcepted(DateTime currentTime) // See exceptions https://www.transportstyrelsen.se/sv/vagtrafik/Trangselskatt/Trangselskatt-i-goteborg/Tider-och-belopp-i-Goteborg/dagar-da-trangselskatt-inte-tas-ut-i-goteborg/
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            if (currentTime.Month == 6)
                return true;

            var customExceptionDays = _IUnitOfWork.FeeExceptionDayRepository.GetAll().Result.Where(
                x => DateTime.Parse(x.Day).Date == currentTime.Date).FirstOrDefault();


            if (customExceptionDays != null)
                return true;

            return false;
        }

        private bool VehicleTypeExcepted(Vehicle vehicle)
        {
            var exceptedVehicleTypes = _IUnitOfWork.FeeExceptionVehicleRepository.GetAll().Result.ToList();

            foreach (var v in exceptedVehicleTypes)
                if (v.VehicleType == vehicle.VehicleType)
                    return true;

            return false;
        }

        private bool HasPassedLessThenOneHour(PassTroughDto PassTrough)
        {
            var listOfRecords = _IUnitOfWork.FeeHeadRepository.GetAll().Result
                .Where(x => x.Name == PassTrough.VehicleFromRegistry.Owner.Name && x.Day.Date == PassTrough.PassTroughTime.Date)
                .Select(x => x.FeeRecords
                .Where(x => x.FeeTime.Date == PassTrough.PassTroughTime.Date &&
                    x.FeeTime.TimeOfDay > PassTrough.PassTroughTime.AddHours(-MaxOneFeePerUnit).TimeOfDay))
                .FirstOrDefault();

            if (listOfRecords != null)
                if (listOfRecords.Count() > 0)
                    return true;

            return false;
        }

        private async void SaveFeeHeadAsync(FeeHead feeHead)
        {
            _IUnitOfWork.FeeHeadRepository.Update(feeHead);
        }

        private FeeHead GetFeeHead(PassTroughDto PassTrough)
        {
            return _IUnitOfWork.FeeHeadRepository.GetAll().Result.Where(x =>
                x.Name == PassTrough.VehicleFromRegistry.Owner.Name &&
                x.Day.Date == PassTrough.PassTroughTime.Date &&
                x.RegNr == PassTrough.VehicleFromRegistry.RegistrationNumber).FirstOrDefault();
        }

        private void AssignFee(PassTroughDto PassTrough)
        {
            int feeAmount = 0;
            PassTrough.VehicleFromRegistry = _vehicleRegistry.GetVehicleByRegNr(PassTrough.InputRegNr);

            if (PassTrough.VehicleFromRegistry == null)
            {
                // Log for statistics
                return;
            }



            var vehicleTypeExcepted = VehicleTypeExcepted(PassTrough.VehicleFromRegistry);
            if (vehicleTypeExcepted)
                return;

            var passedLessThenAnHour = HasPassedLessThenOneHour(PassTrough);
            if (passedLessThenAnHour)
                return;

            var addressException = AddressExcepted(PassTrough);
            if (addressException)
                PassTrough.ExceptionNote = "A";

            var exceptionDay = DateExcepted(PassTrough.PassTroughTime);
            if (exceptionDay)
                PassTrough.ExceptionNote = "H";

            if (string.IsNullOrEmpty(PassTrough.ExceptionNote))
                feeAmount = GetAmountOfFee(PassTrough);

            var feeHead = GetFeeHead(PassTrough);

            if (feeHead != null)
            {
                feeHead.FeeSum = feeHead.FeeSum + feeAmount > MaxFeePerDay ? MaxFeePerDay :
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
