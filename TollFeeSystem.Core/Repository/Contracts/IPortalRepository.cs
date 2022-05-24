using System.Threading.Tasks;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Repository.Contracts
{
    public interface IPortalRepository : IRepository<Portal>
    {
        public Task<Portal> Get(int id);
    }
}
