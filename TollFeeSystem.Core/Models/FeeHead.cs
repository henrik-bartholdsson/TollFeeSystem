﻿using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    public class FeeHead
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FeeRecord> FeeRecords { get; set; }

    }
}
