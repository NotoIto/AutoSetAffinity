using Optional;
namespace Domain
{
    public interface ICPUAffinityRepository
    {
        Option<Process, DomainDefinedError> Update(Process process, CPUAffinity cpuAffinity);
    }
}
