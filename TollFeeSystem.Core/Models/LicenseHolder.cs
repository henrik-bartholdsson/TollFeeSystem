using System.Collections.Generic;
using System.Linq;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Types
{
    public class LicenseHolder
    {
        public string Name { get; set; }
        public List<FeeDay> FeeDays { get; set; }

        public LicenseHolder()
        {
            FeeDays = new List<FeeDay>();
        }
        public void AddFee(FeeRecord fee)
        {
            var feeDay = FeeDays.Where(x => x.Day.Date == fee.FeeTime.Date).FirstOrDefault();

            if (feeDay == null)
                FeeDays.Add(new FeeDay { Fees = new List<FeeRecord> { fee }, Day = fee.FeeTime.Date, SumOfFeeByDay = fee.FeeAmount });
            else
            {
                var sameHour = feeDay.Fees.Where(x => x.FeeTime < fee.FeeTime.AddHours(-1)).FirstOrDefault();

                if(sameHour != null)
                {
                    var a = 0;
                }

                feeDay.Fees.Add(fee);
                
                feeDay.SumOfFeeByDay += fee.FeeAmount;

                if (feeDay.SumOfFeeByDay + fee.FeeAmount > 60)
                    feeDay.SumOfFeeByDay = 60;
                    
            }
        }
    }
}
