namespace TollFeeSystem.Core.Types
{
    public class Vehicle
    {
        public string Brand { get; set; }
        public string RegistrationNumber { get; set; }
        public LicenseHolder Owner { get; set; }
        public VehicleType VehicleType { get; set; }

    }



    public enum VehicleType
    {
        Motorbike,
        Tractor,
        Emergency,
        Diplomat,
        Foreign,
        Military,
        Personal,
        Buss,
    }
}
