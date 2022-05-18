using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Models;
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
            var portalIds = (List<int>)_tollFeeService.GetPortalIds();
            List<string> vehicleRegistry = (List<string>)_tollFeeService.GetVehicleRegistrationNumbers();
            // Byt PassThroughPortal till RegistrerVehicle eller något.
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

            Console.Clear();
            Console.WriteLine("Name \t\t Date \t\t time \t\t Fee \t\t Car reg.nr");
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
                        //if(previouslyDate != "")
                        //Console.WriteLine($"Sum (max 60/day) \t\t\t\t {dayFee}");
                        previouslyDate = y.FeeTime.Date.ToString("yyyy-MM-dd");
                        //dayFee = 0;
                    }
                    Console.CursorLeft = 5;
                    Console.WriteLine($"\t\t\t\t {y.FeeTime.ToString("HH:mm")} \t\t {y.FeeAmount} \t\t {y.VehicleRegistrationNumber}");
                        dayFee += y.FeeAmount;
                        total += y.FeeAmount;

                    //if (previouslyDate != y.FeeTime.Date.ToString("yyyy-MM-dd"))
                    //    Console.WriteLine($"Sum (max 60/day) \t\t\t\t {dayFee}");
                }
                Console.WriteLine($"Total \t\t\t\t\t\t {total}");

            }



        }



    }
}
