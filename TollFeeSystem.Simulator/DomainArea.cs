using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Models;
using System.Linq;
using TollFeeSystem.Core.Types.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace TollFeeSystem.Simulator
{
    public class DomainArea
    {
        private readonly ITollFeeService _tollFeeService;
        public DomainArea()
        {
            var container = Startup.ConfigurService();
            _tollFeeService = container.GetService<ITollFeeService>();
        }


        public void Run()
        {
            //var tollFeeSystem = new TollFeeSystem.Core.TollFeeService();
            var portalIds = (List<int>)_tollFeeService.GetPortalIds();
            List<string> vehicleRegistry = (List<string>)_tollFeeService.GetVehicleRegistrationNumbers();


            _tollFeeService.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-10T07:11:00"), portalIds[7]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-10T16:20:00"), portalIds[0]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-11T07:10:00"), portalIds[1]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-11T16:16:00"), portalIds[0]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-12T07:06:00"), portalIds[2]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-12T16:22:00"), portalIds[5]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T07:12:00"), portalIds[4]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T11:16:00"), portalIds[5]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T12:01:00"), portalIds[3]); // Should be skiped by rule
            _tollFeeService.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-13T12:01:00"), portalIds[3]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-13T16:08:00"), portalIds[5]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-14T10:23:00"), portalIds[1]);
            _tollFeeService.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-14T11:56:00"), portalIds[7]);

            var ownersWithFee = (List<FeeHead>)_tollFeeService.GetLicenseHoldersWithFees();

            ownersWithFee = (List<FeeHead>)ownersWithFee.AsQueryable().OrderBy(x => x.Name).ToList();

            Console.Clear();
            Console.WriteLine("Name \t\t Date \t\t time \t\t Fee \t\t Car reg.nr");
            foreach (var x in ownersWithFee)
            {
                var total = 0;
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(x.Name);
                foreach (var y in x.FeeRecords)
                {
                    Console.WriteLine($"\t\t {y.FeeTime.Date.ToString("yyyy-MM-dd")}");
                    foreach (var z in x.FeeRecords)
                    {
                        Console.WriteLine($"\t\t\t\t {z.FeeTime.ToString("HH:mm")} \t\t {z.FeeAmount} \t\t {z.VehicleRegistrationNumber}");
                    }
                    Console.WriteLine($"Sum (max 60/day) \t\t\t\t {y.FeeAmount}");
                    total += y.FeeAmount;
                }
                Console.WriteLine($"Total \t\t\t\t\t\t {total}");

            }



        }



    }
}
