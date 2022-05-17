using Microsoft.Extensions.DependencyInjection;
using System;
using TollFeeSystem.Core;
using TollFeeSystem.Core.Types.Contracts;

namespace TollFeeSystem.Simulator
{
    public class Startup
    {
        public static IServiceProvider ConfigurService()
        {
            var provider = new ServiceCollection()
                .AddSingleton<ITollFeeService, TollFeeSystem.Core.TollFeeSystemService>()
                .AddScoped<IVehicleRegistry, VehicleRegistryService>()
                .BuildServiceProvider();

            return provider;
        }
    }
}
