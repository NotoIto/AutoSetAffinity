using Domain;
using Optional;
using System;
using System.Management;
namespace Infrastructure
{
    public class CPUInfoSearcherOnDotNet : ICPUInfoSearcher
    {
        public Option<CPUInfo, DomainDefinedError> Read()
        {
            Func<CPUInfo> read = () =>
            {
                var objectSearchQuery = "Select * from Win32_ComputerSystem";
                foreach (var item in new ManagementObjectSearcher(objectSearchQuery).Get())
                {
                    int physicalProcessors = (int)item["NumberOfProcessors"];
                    int logicalProcessors = (int)item["NumberOfLogicalProcessors"];

                    if (physicalProcessors < 1 || logicalProcessors < 1)
                        throw new SystemError("CPUInfoSearcherOnDotNet.Read()", null);

                    return new CPUInfo(
                        PhysicalProcessors: new CPUInfoPhysicalProcessors((uint)physicalProcessors),
                        LogicalProcessors: new CPUInfoLogicalProcessors((uint)logicalProcessors)
                    );
                }
                throw new SystemError("CPUInfoSearcherOnDotNet.Read()", null);
            };
            return read.ToOptionSystemError($"CPUInfoSearcherOnDotNet.Read()");
        }
    }
}
