using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Types.Contracts;

namespace TollFeeSystem.Core.Dto
{
    public class PortalDto
    {
        ITollFeeService _tollFeeService;
        public int PortalId { get; set; }
        public string PortalNameAddress { get; set; }
        public List<string> FeeExceptionsByResidentialAddress { get; set; }

        public PortalDto(ITollFeeService tollFeeService)
        {
            _tollFeeService = tollFeeService;
        }

        public void VehicleInteraction(string regNr, DateTime time)
        {
            _tollFeeService.PassThroughPortal(regNr, time, PortalId);
        }
    }
}
