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
                .AddSingleton<ITollFeeService, TollFeeService>()
                .AddScoped<IVehicleRegistryService, VehicleRegistryService>()
                .BuildServiceProvider();

            return provider;
        }
    }
}
