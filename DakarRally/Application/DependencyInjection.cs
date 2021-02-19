using DakarRally.Application.Interfaces;
using DakarRally.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DakarRally.Application
{
    /// <summary>
    /// Contains the extensions method for registering dependencies in the DI framework.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the necessary services with the DI framework.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRacecService, RacesService>();

            services.AddScoped<IVehiclesService, VehiclesService>();

            services.AddScoped<IRaceDetector, RaceDetector>();

            services.AddScoped<IExceptionLogger, ExceptionLogger>();

            return services;
        }
    }
}
