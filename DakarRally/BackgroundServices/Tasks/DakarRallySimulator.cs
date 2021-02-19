using DakarRally.Application.Interfaces;
using DakarRally.Application.Services;
using DakarRally.Domain.Entities;
using DakarRally.Domain.Enums;
using DakarRally.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRally.BackgroundServices.Tasks
{
    /// <summary>
    /// Dark Rally Simulator background service
    /// </summary>
    [DisallowConcurrentExecution]
    public class DakarRallySimulator : IJob
    {
        private readonly VehicleStatus[] completeStatuses = { VehicleStatus.Broken, VehicleStatus.CompletedRace };


        private readonly ILogger<DakarRallySimulator> _logger;
        private readonly IRacecService _racesService;
        private readonly IDbContext _dbContext;
        private readonly IDateTime _dateTime;
        private readonly Random _random;


        /// <summary>
        /// Initializes a new instance of the <see cref="DakarRallySimulator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="dateTime">The date and time.</param>
        public DakarRallySimulator(ILogger<DakarRallySimulator> logger, IDbContext dbContext, IRacecService racesService, IDateTime dateTime)
        {            
            _logger = logger;
            _dbContext = dbContext;
            _racesService = racesService;
            _dateTime = dateTime;
            _random = new Random();
        }

        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Dakar Rally simulation started!");


            
                _logger.LogInformation("Dakar Rally simulator background service is doing background work.");

                var race = await _dbContext.Set<Race>()
                    .SingleOrDefaultAsync(x => x.Status == RaceStatus.Running);

                if (race != null)
                {
                  await SimulateOneHour(race);
                }

            _logger.LogInformation("Dakar Rally simulation finished!");

        }

        /// <summary>
        /// Simulates one hour of race time.
        /// </summary>
        /// <param name="race">The race that is currently running.</param>
        /// <param name="dbContext">The database context.</param>
        private async Task SimulateOneHour(Race race)
        {
            List<Vehicle> vehicles =  _dbContext.Set<Vehicle>()
               .Include(x => x.Speed)
               .Include(x => x.MalfunctionProbability)
               .Include(x => x.Malfunctions)
               .Where(x => x.RaceId == race.Id)
               .ToList();

            await _racesService.SimulateRaceHour(race, vehicles);

            if (vehicles.All(x => completeStatuses.Contains(x.Status)))
            {
                await _racesService.CompleteRace(race);
            }
             await _dbContext.SaveChangesAsync();
        }
    }
}

