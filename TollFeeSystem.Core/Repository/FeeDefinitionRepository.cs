using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Repository
{
    public class FeeDefinitionRepository : Repository<FeeDefinition>, IFeeDefinitionRepository
    {
        public FeeDefinitionRepository(TfsContext context) : base(context)
        {
        }
    }
}
