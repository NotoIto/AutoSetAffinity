using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Optional;
using Optional.Collections;
using Optional.Linq;
using static Domain.TryUtil;

namespace UseCase
{
    public class SetCPUAffinityService
    {
        protected IConfigRepository configRepository;
        protected ICPUAffinityRepository cpuAffinityRepository;
        protected IProcessSearcher processSearcher;
        protected ICPUInfoSearcher cpuInfoSearcher;

        public Option<Config, DomainDefinedError> GetConfig() =>
            configRepository.Read().Match(
                some: config => Option.Some<Config, DomainDefinedError>(config),
                none: _ => configRepository.Create()
            );
            

        public async Task Delay(Config.PollingInterval pollingInterval, CancellationToken stoppingToken) =>
            await Task.Delay((int)pollingInterval.Value, stoppingToken);

        public Option<Process, DomainDefinedError>[] SetCPUAffinity(Dictionary<ProcessName, Config.ConfigByProcessName> configByProcessNameDictionary) =>
            configByProcessNameDictionary
                .Select(config => processSearcher.FindAllBy(config.Key))
                .Values()
                .SelectMany(_ => _)
                .Select(process =>
                    Try(
                        () => cpuAffinityRepository.Update(process, configByProcessNameDictionary[process.Name].CPUAffinity)
                    )
                    .ToOptionSystemError($"SetCPUAffinityService.SetCPUAffinity({configByProcessNameDictionary})")
                    .Flatten()
                )
                .ToArray();
        
    }
}
