using System.Linq;

namespace Domain
{
    public record CPUAffinity(bool[] Value){
        public ulong ToLong => Value
            .Select((v, i) => v ? 1UL << i : 0)
            .Aggregate(0UL, (sum, next) => sum + next);
    }
}
