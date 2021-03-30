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
using static Domain.ErrorUtil;

namespace UseCase
{
    public class DaemonAppBase
    {
        protected IConfigRepository configRepository;
        protected ICPUAffinityRepository cpuAffinityRepository;
        protected IProcessSearcher processSearcher;
        protected ICPUInfoSearcher cpuInfoSearcher;

        public class AppInitializeError : DomainDefinedError {
            public AppInitializeError(string message) : base($"AppInitializeError({message})", new Exception()) { }
        }

        protected DaemonAppBase() { }

        public static Option<DaemonAppBase, DomainDefinedError> GetInstance() => GetInstanceInternal(new DaemonAppBase());

        protected static Option<T, DomainDefinedError> GetInstanceInternal<T>(T daemonAppBase) where T : DaemonAppBase =>
            from cpuInfo in daemonAppBase.cpuInfoSearcher.Read()
            from _ in cpuInfo switch {
                CPUInfo ci when ci.PhysicalProcessors.Value >= 1 && ci.LogicalProcessors.Value >= 2 => Option.Some<Unit, DomainDefinedError>(new Unit()),
                _ => Option.None<Unit, DomainDefinedError>(new AppInitializeError("Illigal processor(s) count."))
            }
            select daemonAppBase;


        private Option<Config, DomainDefinedError> GetConfig() =>
            configRepository.Read().Match(
                some: config => Option.Some<Config, DomainDefinedError>(config),
                none: _ => configRepository.Create()
            );

        private Option<Unit, DomainDefinedError> Delay(Config.PollingInterval pollingInterval)
        {
            Thread.Sleep((int)pollingInterval.Value);
            return Option.Some<Unit, DomainDefinedError>(new Unit());
        }

        private Option<Process, DomainDefinedError>[] SetCPUAffinity(Dictionary<ProcessName, Config.ConfigByProcessName> configByProcessNameDictionary) =>
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

        public Option<Unit, DomainDefinedError> Execute() =>
            from config in GetConfig()
            from cpuInfo in cpuInfoSearcher.Read()
            from _ in SetCPUAffinity(
                    (
                        from dict in config.configByProcessNameDictionary
                        where dict.Value.CPUAffinity.ToLong >> (int)cpuInfo.LogicalProcessors.Value == 0
                        select dict
                    )
                    .ToDictionary(x => x.Key, x => x.Value)
                )
                .AggregateSystemError()
            from __ in Delay(config.pollingInterval)
            select new Unit();
    }

}
