using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Domain;
using Optional;
using static Domain.TryUtil;
namespace Infrastructure
{
    public class ConfigRepositoryOnJsonFile : JsonFileWrapper, IConfigRepository
    {
        public class ConfigByProcessNameJsonItem
        {
            [JsonPropertyName("cpu_affinity")]
            public readonly bool[] cpuAffinity;

            public ConfigByProcessNameJsonItem(
                bool[] cpuAffinity
                )
            {
                this.cpuAffinity = cpuAffinity;
            }
        }
        public class ConfigJsonItem
        {
            [JsonPropertyName("schema_version")]
            public readonly uint schemaVersion;
            [JsonPropertyName("polling_interval")]
            public readonly uint pollingInterval;
            [JsonPropertyName("by_process")]
            public readonly Dictionary<string, ConfigByProcessNameJsonItem> configByProcessNameDictionary;

            public ConfigJsonItem(
                uint schemaVersion,
                uint pollingInterval,
                 Dictionary<string, ConfigByProcessNameJsonItem> configByProcessNameDictionary
                )
            {
                this.schemaVersion = schemaVersion;
                this.pollingInterval = pollingInterval;
                this.configByProcessNameDictionary = configByProcessNameDictionary;
            }
        }

        public Option<Config, DomainDefinedError> Create() =>
            Try(
                () =>
                    {
                        var config = new Config(
                            schemaVersion: Config.schemaVersion,
                            pollingInterval: new Config.PollingInterval(5000),
                            configByProcessNameDictionary: new Dictionary<ProcessName, Config.ConfigByProcessName>()
                        );
                        var jsonItem = new ConfigJsonItem(
                            schemaVersion: Config.schemaVersion.Value,
                            pollingInterval: config.pollingInterval.Value,
                            configByProcessNameDictionary: config.configByProcessNameDictionary.ToDictionary(
                                configByProcess => configByProcess.Key.Value,
                                configByProcess => new ConfigByProcessNameJsonItem(
                                    cpuAffinity: configByProcess.Value.CPUAffinity.Value
                                )
                            )
                        );
                        var jsonText = Serialize(jsonItem);
                        CreateDirectory();
                        WriteTextFile(jsonText);
                        return config;
                    }
            )
            .ToOptionSystemError("ConfigRepositoryOnJsonFile.Create()");

        public Option<Config, DomainDefinedError> Read() =>
            Try(
                () =>
                    {
                        var jsonText = ReadTextFile();
                        var jsonItem = Deserialize<ConfigJsonItem>(jsonText);
                        return new Config(
                            schemaVersion: new Config.SchemaVersion(jsonItem.schemaVersion),
                            pollingInterval: new Config.PollingInterval(jsonItem.pollingInterval),
                            configByProcessNameDictionary: jsonItem.configByProcessNameDictionary.ToDictionary(
                                configByProcessNameDictonaryJsonItem => new ProcessName(configByProcessNameDictonaryJsonItem.Key),
                                configByProcessNameDictonaryJsonItem => new Config.ConfigByProcessName(
                                    new CPUAffinity(configByProcessNameDictonaryJsonItem.Value.cpuAffinity)
                                )
                            )
                        );
                    }
            )
            .ToOptionSystemError("ConfigRepositoryOnJsonFile.Read()");

        public Option<Config, DomainDefinedError> Update(Config config) =>
            Try(
                () =>
                    {
                        var jsonItem = new ConfigJsonItem(
                            schemaVersion: Config.schemaVersion.Value,
                            pollingInterval: config.pollingInterval.Value,
                            configByProcessNameDictionary: config.configByProcessNameDictionary.ToDictionary(
                                configByProcess => configByProcess.Key.Value,
                                configByProcess => new ConfigByProcessNameJsonItem(
                                    cpuAffinity: configByProcess.Value.CPUAffinity.Value
                                )
                            )
                        );
                        var jsonText = Serialize(jsonItem);
                        WriteTextFile(jsonText);
                        return config;
                    }
            )
            .ToOptionSystemError($"ConfigRepositoryOnJsonFile.Update({config})");
    }
}
