using System;
namespace Domain
{
    public class Config
    {
        public record SchemaVersion(uint Value);
        public record PollingInterval(uint Value);
        public record CPUAffinityByProcess(ProcessName ProcessName, CPUAffinity CPUAffinity);

        public readonly SchemaVersion schemaVersion;
        public readonly PollingInterval pollingInterval;
        public readonly CPUAffinityByProcess[] cpuAffinityByProcesses;

        public Config(SchemaVersion schemaVersion, PollingInterval pollingInterval, CPUAffinityByProcess[] cpuAffinityByProcesses)
        {
            this.schemaVersion = schemaVersion;
            this.pollingInterval = pollingInterval;
            this.cpuAffinityByProcesses = cpuAffinityByProcesses;
        }
    }
}
