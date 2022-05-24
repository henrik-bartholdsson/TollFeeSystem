using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;

namespace TollFeeSystem.Core.Repository
{
    public class FeeExceptionDayRepository : Repository<FeeExceptionDay>, IFeeExceptionDayRepository
    {
        public FeeExceptionDayRepository(TfsContext context) : base(context)
        {
        }
    }
}
