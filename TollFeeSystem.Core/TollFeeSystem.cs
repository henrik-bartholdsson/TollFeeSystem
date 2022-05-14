using System;
using System.Collections.Generic;
using System.Text;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core
{
    internal class TollFeeSystem
    {
        // Main class to interact with the system
        // Use singleton
        // Call portals with vehicle as arguments, the return should may be some kind of message.
        private VehicleRegistry _vehicleRegistry;
        private TollFeeSystem _tollFeeSystem;
        private List<FeeDefinition> _feeDefinitions; // from transportstyrelsen
        private List<DateTime> _FeeExceptionDays;

        public TollFeeSystem(VehicleRegistry vehicleRegistry, TollFeeSystem tollFeeSystem)
        {
            _vehicleRegistry = vehicleRegistry;
            _tollFeeSystem = tollFeeSystem;
        }

        public void PassThroughPortal(Vehicle vehicle, DateTime currentTime)
        {
            Fee fee;
            // Check if vehicle is subject of fee, owner type

            var amountOfFee = GetAmountOfFee(currentTime);
            fee = new Fee { FeeAmount = amountOfFee, FeeTime = currentTime, VehicleRegistrationNumber = "123-ABC" };

            // Assign Fee on vehicle owner
        }








        #region Helpers

        private int GetAmountOfFee(DateTime currentTime)
        {
            var isExcepted = DayIsExceptedFromFee(currentTime);
            if (isExcepted)
                return 0;


            foreach (var fd in _feeDefinitions)
            {
                if (currentTime.TimeOfDay >= fd.Start.TimeOfDay &&
                   currentTime.TimeOfDay <= fd.End.TimeOfDay)
                    return fd.Amount;
            }


            return 0;
        }
        private bool DayIsExceptedFromFee(DateTime currentTime)
        {
            if (_FeeExceptionDays == null)
                return false;

            foreach (var day in _FeeExceptionDays)
            {
                if (day.Date == currentTime.Date)
                    return true;
            }

            return false;
        }


        #endregion
    }
}
