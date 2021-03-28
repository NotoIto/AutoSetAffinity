using System;
using System.Collections.Generic;
namespace Domain
{
    public class Config
    {
        public record SchemaVersion(uint Value);
        public record PollingInterval(uint Value);
        public record ConfigByProcessName(
            CPUAffinity CPUAffinity
        );

        public static readonly SchemaVersion schemaVersion = new SchemaVersion(1);
        public readonly PollingInterval pollingInterval;
        public readonly Dictionary<ProcessName, ConfigByProcessName> configByProcessNameDictionary = new Dictionary<ProcessName, ConfigByProcessName>();

        public class SchemaVersionMismatchException : Exception
        {
            public SchemaVersionMismatchException(SchemaVersion correctSchemaVersion, SchemaVersion givenSchemaVersion)
                :base($"SchemaVersionMismatchException: correctSchemaVersion:{correctSchemaVersion} givenSchemaVersion:{givenSchemaVersion}")
            {
            }
        }
        public Config(SchemaVersion schemaVersion, PollingInterval pollingInterval, Dictionary<ProcessName, ConfigByProcessName> configByProcessNameDictionary)
        {
            if (Config.schemaVersion != schemaVersion)
                throw new SchemaVersionMismatchException(
                    correctSchemaVersion: Config.schemaVersion,
                    givenSchemaVersion: schemaVersion
                );
            this.pollingInterval = pollingInterval;
            this.configByProcessNameDictionary = configByProcessNameDictionary;
        }
    }
}
