using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase
{
    public class ConfigModify
    {
        public class App : SetCPUAffinityDaemon
        {
            public App()
            {
                configRepository = new ConfigRepositoryOnJsonFile();
                processSearcher = new ProcessSearcherOnDotNet();
                cpuInfoSearcher = new CPUInfoSearcherOnDotNet();
            }
        }
        protected IConfigRepository configRepository;
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
