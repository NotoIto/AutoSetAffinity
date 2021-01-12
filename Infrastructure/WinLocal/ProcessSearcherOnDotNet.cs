using Domain;
using System;
using System.Linq;
using Optional;
using DotNetProcess = System.Diagnostics.Process;
using static Domain.TryUtil;

namespace Infrastructure
{
    public class ProcessSearcherOnDotNet : IProcessSearcher
    {
        public Option<Process[], DomainDefinedError> FindAll() =>
            Try(
                () =>
                    DotNetProcess
                        .GetProcesses()
                        .Select(
                            process => new Process(
                                Name: new ProcessName(process.ProcessName),
                                Id: new ProcessId(process.Id)
                            )
                     )
                    .ToArray()
            )
            .ToOptionSystemError("ProcessSearcherOnDotNet.FindAll()");

        public Option<Process[], DomainDefinedError> FindAllBy(ProcessName processName) =>
            Try(
                () =>
                    DotNetProcess
                        .GetProcessesByName(processName.Value)
                        .Select(
                            process => new Process(
                                Name: new ProcessName(process.ProcessName),
                                Id: new ProcessId(process.Id)
                            )
                     )
                    .ToArray()
            )
            .ToOptionSystemError($"ProcessSearcherOnDotNet.FindAllBy({processName})");
    }
}
