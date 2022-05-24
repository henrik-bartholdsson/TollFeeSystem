using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;

namespace TollFeeSystem.Core.Repository
{
    public class PortalRepository : Repository<Portal>, IPortalRepository
    {
        TfsContext _context;
        public PortalRepository(TfsContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Portal> Get(int id)
        {
            return await _context.Portals.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
