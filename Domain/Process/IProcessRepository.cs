using Optional;

namespace Domain
{
    public interface IProcessRepository
    {
        Option<Process[], DomainDefinedError> FindAllBy(ProcessName processName);
        Option<Process[], DomainDefinedError> FindAll();
    }
}
