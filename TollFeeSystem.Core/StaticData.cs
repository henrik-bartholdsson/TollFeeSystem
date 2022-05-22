namespace TollFeeSystem.Core
{
    public static class StaticData
    {
        public enum FeeAmount
        {
            Low = 9,
            Medium = 16,
            High = 22,
        }

        public static int MaxFeePerDay { get; } = 60;
        public static int MaxOneFeePerUnit { get; } = 1;
    }
}
