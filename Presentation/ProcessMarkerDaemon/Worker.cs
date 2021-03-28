using Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UseCase;

namespace ProcessMarkerDaemon
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private class App : SetCPUAffinityService
        {
            public App()
            {
                configRepository = new ConfigRepositoryOnJsonFile();
                cpuAffinityRepository = new CPUAffinityRepositoryOnDotNet();
                processSearcher = new ProcessSearcherOnDotNet();
                cpuInfoSearcher = new CPUInfoSearcherOnDotNet();
            }
        }

        private readonly App app;
        
        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
            app = new App();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                //logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
