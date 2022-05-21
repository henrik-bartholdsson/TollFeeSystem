using Microsoft.EntityFrameworkCore;
using TollFeeSystem.Core.Types;

namespace TollFeeSystem.Core.Models
{
    public class TfsContext : DbContext
    {
        public TfsContext(DbContextOptions<TfsContext> options)
            : base(options) { }

        public DbSet<FeeDefinition> FeeDefinitions { get; set; }
        public DbSet<FeeExceptionDay> FeeExceptionDays { get; set; }
        public DbSet<FeeExceptionsByResidentialAddress> FeeExceptionsByResidentialAddresses { get; set; }
        public DbSet<FeeExceptionVehicle> FeeExceptionVehicles { get; set; }
        public DbSet<FeeHead> FeeHeads { get; set; }
        public DbSet<FeeRecord> FeeRecords { get; set; }
        public DbSet<Portal> Portals { get; set; }
    }
}
