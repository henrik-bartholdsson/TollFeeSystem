using System;

namespace TollFeeSystem.Core.Types
{
    public class FeeDefinition
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
