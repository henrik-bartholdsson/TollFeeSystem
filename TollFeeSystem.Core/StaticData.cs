namespace TollFeeSystem.Core
{
    public static class StaticData
    {
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


        public enum FeeAmount
        {
            Low = 9,
            Medium = 16,
            High = 22,
        }
    }
}
