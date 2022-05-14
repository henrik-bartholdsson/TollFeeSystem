using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;

namespace TollFeeSystem.Core
{
    public class VehicleRegistry : IVehicleRegistry
    {
        public List<Vehicle> Vehicles { get; set; }


        public Vehicle GetVehicle()
        {

            return new Vehicle();
        }

        public bool StoreVehicle(Vehicle vehicle)
        {

            return true;
        }

        public string GetOwnerOfVehicle() // Return Type Owner
        {

            return "";
        }



        private void InformAuthorities() // May be in portal handler
        {
            Console.WriteLine("Unauthorized use of vehicle");
        }
    }
}
