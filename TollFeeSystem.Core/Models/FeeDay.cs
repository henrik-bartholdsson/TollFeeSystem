using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    public class FeeDay
    {
        public DateTime Day { get; set; }
        public List<FeeRecord> Fees { get; set; }
        public int SumOfFeeByDay { get; set; }
    }
}
