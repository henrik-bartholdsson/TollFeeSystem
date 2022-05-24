namespace TollFeeSystem.Core.Repository.Contracts
{
    public interface IUnitOfWork
    {
        IFeeExceptionVehicleRepository FeeExceptionVehicleRepository { get; set; }
        IFeeExceptionDayRepository FeeExceptionDayRepository { get; set; }
        IFeeDefinitionRepository FeeDefinitionRepository { get; set; }
        IPortalRepository PortalRepository { get; set; }
        IFeeHeadRepository FeeHeadRepository { get; set; }
    }
}
