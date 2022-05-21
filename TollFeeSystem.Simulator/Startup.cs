using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using TollFeeSystem.Core;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Types.Contracts;

namespace TollFeeSystem.Simulator
{
    public class Startup
    {
        public static IServiceProvider ConfigurService()
        {
            var provider = new ServiceCollection()
                .AddSingleton<ITollFeeService, TollFeeService>()
                .AddScoped<IVehicleRegistryService, VRService>()
                .AddDbContext<TfsContext>(options =>
                {
                    options.UseInMemoryDatabase("TollFeeDatabase");
                })
                .BuildServiceProvider();

            return provider;
        }
    }
}
