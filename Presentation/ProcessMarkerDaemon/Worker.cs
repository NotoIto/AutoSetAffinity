using Domain;
using Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Optional;
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

        private class App : DaemonAppBase
        {
            protected App()
            {
                configRepository = new ConfigRepositoryOnJsonFile();
                cpuAffinityRepository = new CPUAffinityRepositoryOnDotNet();
                processSearcher = new ProcessSearcherOnDotNet();
                cpuInfoSearcher = new CPUInfoSearcherOnDotNet();
            }

            public new static Option<App, DomainDefinedError> GetInstance() => GetInstanceInternal(new App());
        }

        private readonly App app;
        
        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
            app = App.GetInstance().Match(
                some: app => app,
                none: e => throw e
                );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await new Task(
                    () =>
                        app.Execute().MatchNone(
                            e => logger.LogError(e.Message, e)
                        )
                );
            }
        }
    }
}
