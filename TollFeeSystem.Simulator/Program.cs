using System;
using TollFeeSystem.Core;
using TollFeeSystem.Core.Types.Contracts;
using Microsoft.Extensions.DependencyInjection;

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

            gothenburg.Run();
        }

    }
}
