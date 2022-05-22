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

            //_portals[7].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-04-10T07:11:00"));
            //_portals[7].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-04-10T09:02:00"));
            //_portals[0].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-10T16:20:00"));
            //_portals[1].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-11T07:10:00"));
            //_portals[0].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-11T16:16:00"));
            //_portals[4].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-12T07:06:00"));
            //_portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T08:08:00"));
            //_portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T09:09:00"));
            //_portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T10:10:00"));
            //_portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T16:10:00"));
            //_portals[2].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-12T17:11:00"));
            //_portals[5].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-12T16:22:00"));
            //_portals[6].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-13T07:12:00"));
            //_portals[5].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-13T11:16:00"));
            //_portals[3].VehicleInteraction(vehicleRegistry[1], DateTime.Parse("2022-05-13T12:01:00"));
            //_portals[3].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-13T12:01:00"));
            //_portals[3].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-13T12:30:00")); // Will be excluded due to 1 hour rule
            //_portals[5].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-22T16:08:00")); // Will be excluded because it's sunday
            //_portals[1].VehicleInteraction(vehicleRegistry[2], DateTime.Parse("2022-05-23T10:23:00"));
            //_portals[7].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2022-05-23T10:56:00"));
            _portals[7].VehicleInteraction(vehicleRegistry[0], DateTime.Parse("2021-12-24T12:56:00"));

            var ownersWithFee = _tfService.GetLicenseHoldersWithFeesAwait().Result.ToList();

            Console.Clear();
            Console.WriteLine("Name \t\t Date \t\t Time \t\t Reg.nr \t Sum (SEK)  Note");
            foreach (var h in ownersWithFee)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.Write($"{h.Name}");
                Console.CursorLeft = 65;
                Console.Write(h.FeeSum);
                Console.CursorLeft = 17;
                Console.Write(h.Day.Date.ToString("yyyy-MM-dd"));

                foreach (var r in h.FeeRecords)
                {
                    if (h.FeeRecords.Count == 1)
                    {
                        Console.CursorLeft = 76;
                        Console.Write(r.ExceptionNote);
                        Console.CursorLeft = 25;
                        Console.WriteLine($"\t {h.Day.ToString("HH:mm")} \t\t {h.RegNr}");
                        break;
                    }
                    else
                    {
                        Console.CursorLeft = 76;
                        Console.Write(r.ExceptionNote);
                        Console.CursorLeft = 33;
                        Console.WriteLine($"{r.FeeTime.ToString("HH:mm")} \t\t {r.VehicleRegistrationNumber}");
                    }

                }
                Console.WriteLine();
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