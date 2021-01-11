using Optional;

namespace Domain
{
    public interface ICPUInfoSearcher
    {
        Option<CPUInfo, DomainDefinedError> Read();
    }
}
