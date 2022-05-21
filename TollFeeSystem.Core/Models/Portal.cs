using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollFeeSystem.Core.Models
{
    public class Portal
    {
        public int Id { get; set; }
        public string PortalNameAddress { get; set; }
        [ForeignKey("Id")]
        public List<FeeExceptionsByResidentialAddress> FeeExceptionsByResidentialAddress { get; set; }
    }
}
