using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Optional;
using Optional.Linq;

namespace Application
{
    public class SetCPUAffinityDaemon
    {
        public class App : SetCPUAffinityDaemon
        {
            public App()
            {
                configRepository = new ConfigRepositoryOnJsonFile();
                cpuAffinityRepository = new CPUAffinityRepositoryOnDotNet();
                processSearcher = new ProcessSearcherOnDotNet();
                cpuInfoSearcher = new CPUInfoSearcherOnDotNet();
            }
        }
        protected IConfigRepository configRepository;
        protected ICPUAffinityRepository cpuAffinityRepository;
        protected IProcessSearcher processSearcher;
        protected ICPUInfoSearcher cpuInfoSearcher;

        /*public Option<int, DomainDefinedError> Execute()
        {
            var result =
                from config in configRepository.Read()//TODO
                from cpuInfo in cpuInfoRepository.Read()
        }*/
    }
}
