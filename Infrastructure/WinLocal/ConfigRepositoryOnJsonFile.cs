using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Optional;
namespace Infrastructure
{
    public class ConfigRepositoryOnJsonFile : JsonFileWrapper, IConfigRepository
    {
        public class CPUAffinityByProcessJsonItem
        {
            [JsonPropertyName("process_name")]
            public readonly string processName;
            [JsonPropertyName("cpu_affinity")]
            public readonly bool[] cpuAffinity;

            public CPUAffinityByProcessJsonItem(
                string processName,
                bool[] cpuAffinity
                )
            {
                this.processName = processName;
                this.cpuAffinity = cpuAffinity;
            }
        }
        public class ConfigJsonItem
        {
            [JsonPropertyName("schema_version")]
            public readonly uint schemaVersion;
            [JsonPropertyName("polling_interval")]
            public readonly uint pollingInterval;
            [JsonPropertyName("cpu_affinity_by_process")]
            public readonly CPUAffinityByProcessJsonItem[] cpuAffinityByProcesses;

            public ConfigJsonItem(
                uint schemaVersion,
                uint pollingInterval,
                CPUAffinityByProcessJsonItem[] cpuAffinityByProcesses
                )
            {
                this.schemaVersion = schemaVersion;
                this.pollingInterval = pollingInterval;
                this.cpuAffinityByProcesses = cpuAffinityByProcesses;
            }
        }

        public Option<Config, DomainDefinedError> Create()
        {
            Func<Config> create = () =>
            {
                var config = new Config(
                    schemaVersion: new Config.SchemaVersion(1),
                    pollingInterval: new Config.PollingInterval(5),
                    cpuAffinityByProcesses: new Config.CPUAffinityByProcess[0]
                );
                var jsonItem = new ConfigJsonItem(
                    schemaVersion: config.schemaVersion.Value,
                    pollingInterval: config.pollingInterval.Value,
                    cpuAffinityByProcesses: config.cpuAffinityByProcesses.Select(
                        cpuAffinityByProcess => new CPUAffinityByProcessJsonItem(
                            processName: cpuAffinityByProcess.ProcessName.Value,
                            cpuAffinity: cpuAffinityByProcess.CPUAffinity.Value
                        )
                    )
                    .ToArray()
                );
                var jsonText = Serialize(jsonItem);
                CreateDirectory();
                WriteTextFile(jsonText);
                return config;
            };

            return create.ToOptionSystemError("ConfigRepositoryOnJsonFile.Create()");
        }

        public Option<Config, DomainDefinedError> Read()
        {
            Func<Config> read = () =>
            {
                var jsonText = ReadTextFile();
                var jsonItem = Deserialize<ConfigJsonItem>(jsonText);
                return new Config(
                    schemaVersion: new Config.SchemaVersion(jsonItem.schemaVersion),
                    pollingInterval: new Config.PollingInterval(jsonItem.pollingInterval),
                    cpuAffinityByProcesses: jsonItem.cpuAffinityByProcesses.Select(
                        cpuAffinityByProcessJsonItem =>
                            new Config.CPUAffinityByProcess(
                                new ProcessName(cpuAffinityByProcessJsonItem.processName),
                                new CPUAffinity(cpuAffinityByProcessJsonItem.cpuAffinity)
                            )
                    )
                    .ToArray()
                );
            };
            return read.ToOptionSystemError("ConfigRepositoryOnJsonFile.Read()");
        }

        public Option<Config, DomainDefinedError> Update(Config config)
        {
            Func<Config> update = () =>
            {
                var jsonItem = new ConfigJsonItem(
                    schemaVersion: config.schemaVersion.Value,
                    pollingInterval: config.pollingInterval.Value,
                    cpuAffinityByProcesses: config.cpuAffinityByProcesses.Select(
                        cpuAffinityByProcess => new CPUAffinityByProcessJsonItem(
                            processName: cpuAffinityByProcess.ProcessName.Value,
                            cpuAffinity: cpuAffinityByProcess.CPUAffinity.Value
                        )
                    )
                    .ToArray()
                );
                var jsonText = Serialize(jsonItem);
                WriteTextFile(jsonText);
                return config;
            };
            return update.ToOptionSystemError($"ConfigRepositoryOnJsonFile.Update({config})");
        }
    }
}
