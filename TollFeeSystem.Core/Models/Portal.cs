using System.Collections.Generic;

namespace TollFeeSystem.Core.Models
{
    internal class Portal
    {
        public int PortalId { get; set; }
        public string PortalNameAddress { get; set; }
        public List<string> FeeExceptionsByResidentialAddress { get; set; }
    }
}
