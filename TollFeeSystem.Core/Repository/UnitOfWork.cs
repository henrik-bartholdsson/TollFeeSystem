using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository.Contracts;

namespace TollFeeSystem.Core.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TfsContext _context;
        public UnitOfWork(TfsContext context)
        {
            _context = context;
            FeeExceptionVehicleRepository = new FeeExceptionVehicleRepository(_context);
            FeeExceptionDayRepository = new FeeExceptionDayRepository(_context);
            FeeDefinitionRepository = new FeeDefinitionRepository(_context);
            PortalRepository = new PortalRepository(_context);
            FeeHeadRepository = new FeeHeadRepository(_context);
        }

        public IFeeExceptionVehicleRepository FeeExceptionVehicleRepository { get; set; }
        public IFeeExceptionDayRepository FeeExceptionDayRepository { get; set; }
        public IFeeDefinitionRepository FeeDefinitionRepository { get; set; }
        public IPortalRepository PortalRepository { get; set; }
        public IFeeHeadRepository FeeHeadRepository { get; set; }
    }
}
