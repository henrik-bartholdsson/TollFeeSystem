using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;

namespace TollFeeSystem.Core.Repository
{


    public class FeeHeadRepository : Repository<FeeHead>, IFeeHeadRepository
    {
        private readonly TfsContext _context;

        public FeeHeadRepository(TfsContext context) : base(context)
        {
            _context = context;
        }

        public async void UpdateAsync(FeeHead feeHead)
        {
            _context.FeeHeads.Update(feeHead);
            await _context.SaveChangesAsync();
        }
    }
}
