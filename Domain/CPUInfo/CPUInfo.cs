using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public record CPUInfoPhysicalProcessors(uint Value);
    public record CPUInfoLogicalProcessors(uint Value);

    public record CPUInfo(
        CPUInfoPhysicalProcessors PhysicalProcessors,
        CPUInfoLogicalProcessors LogicalProcessors
    );
}
