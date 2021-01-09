using Domain;
using System;
using System.Linq;
using Optional;
using DotNetProcess = System.Diagnostics.Process;

namespace Infrastructure
{
    public class ProcessRepositoryOnDotNet : IProcessRepository
    {
        public Option<Process[], DomainDefinedError> FindAll()
        {
            Func<Process[]> findAll = () =>
                DotNetProcess
                .GetProcesses()
                .Select(
                    p => new Process(
                        Name: new ProcessName(p.ProcessName),
                        Id: new ProcessId(p.Id)
                    )
                 )
                .ToArray();
            return findAll.ToOptionSystemError("ProcessRepositoryOnDotNet.FindAll()");
        }

        public Option<Process[], DomainDefinedError> FindAllBy(ProcessName processName)
        {
            Func<Process[]> findAllBy = () =>
                DotNetProcess
                .GetProcessesByName(processName.Value)
                .Select(
                    p => new Process(
                        Name: new ProcessName(p.ProcessName),
                        Id: new ProcessId(p.Id)
                    )
                 )
                .ToArray();
            return findAllBy.ToOptionSystemError($"ProcessRepositoryOnDotNet.FindAllBy({processName})");
        }
    }
}
