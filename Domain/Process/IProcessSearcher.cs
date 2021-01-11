using Optional;

namespace Domain
{
    public interface IProcessSearcher
    {
        Option<Process[], DomainDefinedError> FindAllBy(ProcessName processName);
        Option<Process[], DomainDefinedError> FindAll();
    }
}
