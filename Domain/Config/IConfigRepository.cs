using Optional;
namespace Domain
{
    public interface IConfigRepository
    {
        Option<Config, DomainDefinedError> Create();
        Option<Config, DomainDefinedError> Read();
        Option<Config, DomainDefinedError> Update(Config config);

    }
}
