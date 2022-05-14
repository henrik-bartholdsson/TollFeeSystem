using System;
using System.Collections.Generic;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core
{
    public class VehicleRegistry
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

        public string GetOwnerOfVehicle() // Type Owner
        {

            return "";
        }



        private void InformAuthorities() // May be in portal handler
        {
            Console.WriteLine("Unauthorized use of vehicle");
        }
    }
}
