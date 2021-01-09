using Domain;
using Optional;

namespace Infrastructure
{
    public class CPUInfoRepositoryOnDotNet : ICPUInfoRepository
    {
        public Option<CPUInfo, DomainDefinedError> Read()
        {
            throw new System.NotImplementedException();
        }
    }
}
