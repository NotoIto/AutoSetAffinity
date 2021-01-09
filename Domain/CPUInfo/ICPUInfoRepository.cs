using Optional;

namespace Domain
{
    public interface ICPUInfoRepository
    {
        Option<CPUInfo, DomainDefinedError> Read();
    }
}
