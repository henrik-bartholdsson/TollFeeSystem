using System.Collections.Generic;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Core.Types
{
    public class VehicleOwner
    {
        public string Name { get; set; }
        public List<Fee> Fees { get; set; }

        public VehicleOwner()
        {
            Fees = new List<Fee>();
        }
        public void AddFee(Fee fee)
        {
            Fees.Add(fee);
        }
    }
}
