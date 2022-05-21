using TollFeeSystem.Core;
using TollFeeSystem.Core.Types.Contracts;
using Microsoft.Extensions.DependencyInjection;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var container = Startup.ConfigurService();

            var gothenburg = new DomainArea(
                container.GetService<ITollFeeService>(),
                container.GetService<IVehicleRegistryService>()
                );

            var ts = new TestService(container.GetService<TfsContext>());

            ts.AddFeeRecord();


            var records = ts.GetRecords();


            gothenburg.Run();
        }

    }
}
