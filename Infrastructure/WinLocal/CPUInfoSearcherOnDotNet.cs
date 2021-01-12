using Domain;
using Optional;
using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using Optional.Linq;
using static Domain.TryUtil;
namespace Infrastructure
{
    public class CPUInfoSearcherOnDotNet : ICPUInfoSearcher
    {
        public Option<CPUInfo, DomainDefinedError> Read() =>
            (
                new ManagementObjectSearcher("Select * from Win32_ComputerSystem")
                    .Get()
                    as IEnumerable<ManagementBaseObject>
            )
            .Select(
                searchResult =>
                    Try(
                        () => (
                            (int)searchResult["NumberOfProcessors"],
                            (int)searchResult["NumberOfLogicalProcessors"]
                        )
                    )
                    .ToOptionSystemError($"CPUInfoSearcherOnDotNet.Read()")
            )
            .Aggregate((first, _) => first)
            .SelectMany(
                physicalAndLogicalProcessors => physicalAndLogicalProcessors switch
                {
                    (int physicalProcessors, int logicalProcessors)
                        when physicalProcessors > 0 && logicalProcessors > 0 =>
                            Option.Some<CPUInfo, DomainDefinedError>(
                                new CPUInfo(
                                    PhysicalProcessors: new CPUInfoPhysicalProcessors((uint)physicalProcessors),
                                    LogicalProcessors: new CPUInfoLogicalProcessors((uint)logicalProcessors)
                                )
                            ),
                    _ => Option.None<CPUInfo, DomainDefinedError>(
                        new SystemError($"CPUInfoSearcherOnDotNet.Read()", null)
                    )
                }
            );
    }
}
