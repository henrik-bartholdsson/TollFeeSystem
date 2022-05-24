using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Repository.Contracts
{
    public interface IFeeHeadRepository : IRepository<FeeHead>
    {
        public void Update(FeeHead feeHead);
    }
}
