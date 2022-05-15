using System;
using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tollFeeSystem = new TollFeeSystem.Core.TollFeeSystem();
            List<string> vehicleRegistry = (List<string>) tollFeeSystem.GetAllVehicleRegistrationNumbers();

            tollFeeSystem.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-10T07:11:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-10T16:20:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-11T07:10:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-11T16:16:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[0], DateTime.Parse("2022-05-12T07:06:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-12T16:22:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T07:12:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T11:16:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[1], DateTime.Parse("2022-05-13T12:01:00")); // Should be skiped by rule
            tollFeeSystem.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-13T12:01:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-13T16:08:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-14T10:23:00"));
            tollFeeSystem.PassThroughPortal(vehicleRegistry[2], DateTime.Parse("2022-05-14T11:56:00"));

            var ownersWithFee = (List<FeeHead>) tollFeeSystem.GetLicenseHoldersThatHaveFees();

            ownersWithFee = (List<FeeHead>) ownersWithFee.AsQueryable().OrderBy(x => x.Name).ToList();

            Console.Clear();
            Console.WriteLine("Name \t\t Date \t\t time \t\t Fee \t\t Car reg.nr");
            foreach(var x in ownersWithFee)
            {
                var total = 0;
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(x.Name);
                foreach(var y in x.FeeRecords)
                {
                    Console.WriteLine($"\t\t {y.FeeTime.Date.ToString("yyyy-MM-dd")}");
                    foreach(var z in x.FeeRecords)
                    {
                        Console.WriteLine($"\t\t\t\t {z.FeeTime.ToString("HH:mm")} \t\t {z.FeeAmount} \t\t {z.VehicleRegistrationNumber}");
                    }
                    Console.WriteLine($"Sum (max 60/day) \t\t\t\t {y.FeeAmount}");
                    total += y.FeeAmount;
                }
                Console.WriteLine($"Total \t\t\t\t\t\t {total}");

            }

            //var menu = new UImenu();

            //menu.Run();



            Console.ReadKey();

        }

    }
}
