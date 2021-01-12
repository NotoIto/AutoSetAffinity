using Domain;
using static Domain.TryUtil;
using System;
using Optional;
using System.Linq;
using System.Collections.Generic;
using ProcessThread =  System.Diagnostics.ProcessThread;
using DotNetProcess = System.Diagnostics.Process;
namespace Infrastructure
{
    public class CPUAffinityRepositoryOnDotNet : ICPUAffinityRepository
    {
        public Option<CPUAffinity, DomainDefinedError> Update(CPUAffinity cpuAffinity, Process process) =>
            Try(
                () =>
                    {
                        (
                            DotNetProcess
                                .GetProcessById(process.Id.Value)
                                .Threads
                                as IEnumerable<ProcessThread>
                        )
                        .ToList() //TODO: List.ForEachとSelectどちらが速いのか検証する
                        .ForEach(
                            thread => thread.ProcessorAffinity = (IntPtr)cpuAffinity.ToLong
                        );
                        return cpuAffinity;
                    }
            )
            .ToOptionSystemError($"CPUAffinityRepositoryOnDotNet.Update({cpuAffinity}, {process})");
    }
}
