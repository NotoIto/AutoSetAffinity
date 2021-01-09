using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public record CPUInfoThreadCount(int Value);

    public record CPUInfo(
        CPUInfoThreadCount threadCount
    );
}
