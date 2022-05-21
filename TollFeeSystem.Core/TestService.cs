using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core
{
    public class TestService
    {
        private readonly TfsContext _tfsContext;
        public TestService(TfsContext tfsContext)
        {
            _tfsContext = tfsContext;
        }

        public void AddFeeRecord()
        {
            _tfsContext.FeeRecords.Add(
                new FeeRecord { FeeAmount = 123, FeeTime = DateTime.Now, VehicleRegistrationNumber = "ABS-321" });
            _tfsContext.SaveChanges();
        }


        public async Task<IEnumerable<FeeRecord>> GetRecords()
        {
            return await _tfsContext.FeeRecords.Where(x => x != null).ToListAsync();
        }
    }
}
