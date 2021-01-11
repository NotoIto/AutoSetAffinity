using Domain;
using System;
using Optional;
using DotNetProcess = System.Diagnostics.Process;

namespace Infrastructure
{
    public class CPUAffinityRepositoryOnDotNet : ICPUAffinityRepository
    {
        public Option<CPUAffinity, DomainDefinedError> Update(CPUAffinity cpuAffinity, Process process)
        {
            Func<CPUAffinity> update = () =>
            {
                foreach (System.Diagnostics.ProcessThread thread in DotNetProcess.GetProcessById(process.Id.Value).Threads)
                {
                    thread.ProcessorAffinity = (IntPtr)cpuAffinity.ToLong;
                }
                return cpuAffinity;
            };
            return update.ToOptionSystemError($"CPUAffinityRepositoryOnDotNet.Update({cpuAffinity}, {process})");
        }
    }
}
