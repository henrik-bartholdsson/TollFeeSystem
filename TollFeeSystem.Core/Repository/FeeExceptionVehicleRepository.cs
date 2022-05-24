using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;

namespace TollFeeSystem.Core.Repository
{
    public class FeeExceptionVehicleRepository : Repository<FeeExceptionVehicle>, IFeeExceptionVehicleRepository
    {
        public FeeExceptionVehicleRepository(TfsContext context) : base(context)
        {

        }
    }
}