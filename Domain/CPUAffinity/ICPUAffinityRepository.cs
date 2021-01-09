using Optional;
namespace Domain
{
    public interface ICPUAffinityRepository
    {
        Option<CPUAffinity, DomainDefinedError> Update(CPUAffinity cpuAffinity, Process process);
    }
}
