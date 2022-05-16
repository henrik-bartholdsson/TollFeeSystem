using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Types;
using TollFeeSystem.VehicleRegistry.Models;

namespace TollFeeSystem.Core
{
    public interface IVehicleRegistry
    {
        List<string> GetAllVehicleRegistrationNumbers();
        Vehicle GetVehicleByRegNr(string regNr);
    }

    public class VehicleRegistry : IVehicleRegistry
    {
        private VRContext _VrContext;

        public VehicleRegistry()
        {
            _VrContext = new VRContext();
            Initialize();
        }

        public Vehicle GetVehicleByRegNr(string regNr)
        {
            return _VrContext.Vehicles.FirstOrDefault(x => x.RegistrationNumber == regNr);
        }

        public List<string> GetAllVehicleRegistrationNumbers()
        {
            return _VrContext.Vehicles.Select(x => x.RegistrationNumber).ToList();
        }




        #region Private methods
        private void Initialize()
        {
            _VrContext.LicenseHolders = new List<LicenseHolder>();
            _VrContext.LicenseHolders.Add(new LicenseHolder() { Name = "Neo", });
            _VrContext.LicenseHolders.Add(new LicenseHolder() { Name = "Ripley", });
            _VrContext.LicenseHolders.Add(new LicenseHolder() { Name = "Elizabeth.", });
            _VrContext.LicenseHolders.Add(new LicenseHolder() { Name = "Luke", });
            _VrContext.LicenseHolders.Add(new LicenseHolder() { Name = "Bilbo", });

            _VrContext.Vehicles = new List<Vehicle>();
            _VrContext.Vehicles.Add(new Vehicle() { Brand = "Volvo 142", Owner = _VrContext.LicenseHolders[0], RegistrationNumber = "ABC-123", VehicleType = VehicleType.Personal });
            _VrContext.Vehicles.Add(new Vehicle() { Brand = "WV Golf GTI", Owner = _VrContext.LicenseHolders[1], RegistrationNumber = "JSK-983", VehicleType = VehicleType.Personal });
            _VrContext.Vehicles.Add(new Vehicle() { Brand = "Scoda Fabia", Owner = _VrContext.LicenseHolders[2], RegistrationNumber = "BWU-081", VehicleType = VehicleType.Personal });
            _VrContext.Vehicles.Add(new Vehicle() { Brand = "Testa S2", Owner = _VrContext.LicenseHolders[3], RegistrationNumber = "PCE-592", VehicleType = VehicleType.Personal });
            _VrContext.Vehicles.Add(new Vehicle() { Brand = "Testa S2", Owner = _VrContext.LicenseHolders[0], RegistrationNumber = "PCF-591", VehicleType = VehicleType.Personal });
        }



        #endregion
    }
}
