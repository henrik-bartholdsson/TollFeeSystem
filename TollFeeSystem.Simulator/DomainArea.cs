using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types.Contracts;
using TollFeeSystem.Core;
using TollFeeSystem.Core.Dto;
using System.Linq;

namespace TollFeeSystem.Simulator
{
    public class DomainArea
    {
        private readonly IVehicleRegistryService _vrService;
        private readonly ITollFeeService _tfService;
        private List<PortalDto> _portals;

        public DomainArea(ITollFeeService tfService, IVehicleRegistryService vrService)
        {
            _tfService = tfService;
            _vrService = vrService;

            Init(_tfService);
        }


        public void Run()
        {
            List<string> vehicleRegistry = (List<string>)_vrService.GetVehicleRegistrationNumbers();

            _portals[7].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-10T07:11:00"));
            _portals[0].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-10T16:20:00"));
            _portals[1].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-11T07:10:00"));
            _portals[0].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-11T16:16:00"));
            _portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T07:06:00"));
            _portals[5].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-12T16:22:00"));
            _portals[4].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-13T07:12:00"));
            _portals[5].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-13T11:16:00"));
            _portals[3].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-13T12:01:00"));
            _portals[3].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-13T12:01:00"));
            _portals[5].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-13T16:08:00"));
            _portals[1].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-14T10:23:00"));
            _portals[7].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-14T11:56:00"));

            var ownersWithFee = (List<FeeHead>)_tfService.GetLicenseHoldersWithFees();

            Console.Clear();
            Console.WriteLine("Name \t\t Date \t\t Time \t\t Fee (SEK) \t\t Reg.nr");
            foreach (var x in ownersWithFee)
            {
                var previouslyDate = "";
                var total = 0;
                var dayFee = 0;
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.Write(x.Name);
                foreach (var y in x.FeeRecords) // Jag måste separera dagarna på något bra sätt...
                {
                    if(previouslyDate != y.FeeTime.Date.ToString("yyyy-MM-dd"))
                    {
                        Console.CursorLeft = 7;
                        Console.Write($"\t\t {y.FeeTime.Date.ToString("yyyy-MM-dd")}");
                        previouslyDate = y.FeeTime.Date.ToString("yyyy-MM-dd");
                    }
                    Console.CursorLeft = 5;
                    Console.WriteLine($"\t\t\t\t {y.FeeTime.ToString("HH:mm")} \t\t {y.FeeAmount} \t\t {y.VehicleRegistrationNumber}");
                        dayFee += y.FeeAmount;
                        total += y.FeeAmount;
                }
                Console.WriteLine($"Total \t\t\t\t\t\t {total}");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("End of simulation...");
            Console.ResetColor();
        }

        private void Init(ITollFeeService _tollFeeService)
        {
            var portsData = _tfService.GetPortalsAsync().Result;
            _portals = new List<PortalDto>();

            foreach (var p in portsData)
                _portals.Add(
                    new PortalDto(_tollFeeService)
                    {
                        FeeExceptionsByResidentialAddress = GetAddresses(p),
                        PortalNameAddress = p.PortalNameAddress,
                        PortalId = p.Id
                    });


        }

        private List<string> GetAddresses(Portal p)
        {
            if(p.FeeExceptionsByResidentialAddress != null)
            return p.FeeExceptionsByResidentialAddress.Select(x => x.Address).ToList();

            return new List<string>();
        }

    }
}


// Måste knyta portal till adress som inte ska ha avgift