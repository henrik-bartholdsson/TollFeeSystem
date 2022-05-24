using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TollFeeSystem.Core;
using TollFeeSystem.Core.Models;
using TollFeeSystem.Core.Repository;
using TollFeeSystem.Core.Repository.Contracts;
using TollFeeSystem.Core.Service;
using TollFeeSystem.Core.Types;
using TollFeeSystem.Core.Types.Contracts;
using static TollFeeSystem.Core.StaticData;

namespace TollFeeSystem.Simulator
{
    public class Startup
    {
        public static IServiceProvider ConfigurService()
        {
            var provider = new ServiceCollection()
                .AddSingleton<ITollFeeService, TollFeeService>()
                .AddScoped<IVehicleRegistryService, VRService>()
                .AddTransient<IFeeExceptionVehicleRepository, FeeExceptionVehicleRepository>()
                .AddTransient<IFeeExceptionDayRepository, FeeExceptionDayRepository>()
                .AddTransient<IFeeDefinitionRepository, FeeDefinitionRepository>()
                .AddTransient<IPortalRepository, PortalRepository>()
                .AddTransient<IFeeHeadRepository, FeeHeadRepository>()
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddDbContext<TfsContext>(options =>
                {
                    options.UseInMemoryDatabase("TollFeeDatabase");
                })
                .BuildServiceProvider();

            InitDatabase(provider.GetRequiredService<TfsContext>());

            return provider;
        }


        private static void InitDatabase(TfsContext _TFSContext)
        {
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:00:00"), End = DateTime.Parse("06:29:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("06:30:00"), End = DateTime.Parse("06:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("07:00:00"), End = DateTime.Parse("07:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:00:00"), End = DateTime.Parse("08:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("08:30:00"), End = DateTime.Parse("14:59:59"), Amount = (int)FeeAmount.Low });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:00:00"), End = DateTime.Parse("15:29:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("15:30:00"), End = DateTime.Parse("16:59:59"), Amount = (int)FeeAmount.High });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("17:00:00"), End = DateTime.Parse("17:59:59"), Amount = (int)FeeAmount.Medium });
            _TFSContext.FeeDefinitions.Add(new FeeDefinition { Start = DateTime.Parse("18:00:00"), End = DateTime.Parse("18:29:59"), Amount = (int)FeeAmount.Low });

            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Emergency });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Buss });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Diplomat });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Motorbike });
            _TFSContext.FeeExceptionVehicles.Add(new FeeExceptionVehicle { VehicleType = VehicleType.Military });

            _TFSContext.FeeExceptionDays.Add(new FeeExceptionDay { Day = "2021-12-24" });

            var feeExceptionsByResidentialAddress1 = new List<FeeExceptionsByResidentialAddress>()
            { new FeeExceptionsByResidentialAddress { Address = "Backa" } };

            var feeExceptionsByResidentialAddress2 = new List<FeeExceptionsByResidentialAddress>()
            { new FeeExceptionsByResidentialAddress { Address = "Backa" } };

            _TFSContext.Portals.Add(new Portal() { Id = 11, PortalNameAddress = "Tingstad North" });
            _TFSContext.Portals.Add(new Portal() { Id = 12, PortalNameAddress = "Tingstad South" });
            _TFSContext.Portals.Add(new Portal() { Id = 13, PortalNameAddress = "E6 North Entrance" });
            _TFSContext.Portals.Add(new Portal() { Id = 14, PortalNameAddress = "E6 North Departure" });
            _TFSContext.Portals.Add(new Portal() { Id = 15, PortalNameAddress = "E6 South Entrance" });
            _TFSContext.Portals.Add(new Portal() { Id = 16, PortalNameAddress = "E6 South Departure" });
            _TFSContext.Portals.Add(new Portal()
            {
                Id = 17,
                PortalNameAddress = "Backa Entrance",
                FeeExceptionsByResidentialAddress = feeExceptionsByResidentialAddress1
            });
            _TFSContext.Portals.Add(new Portal()
            {
                Id = 18,
                PortalNameAddress = "Backa Departure",
                FeeExceptionsByResidentialAddress = feeExceptionsByResidentialAddress2
            });

            _TFSContext.SaveChanges();
        }
    }
}
